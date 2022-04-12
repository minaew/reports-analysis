using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UglyToad.PdfPig;

namespace PdfExtractor
{
    public class SberParser : IParser
    {
        public IReadOnlyList<Operation> Parse(string path)
        {
            var operations = new List<Operation>();

            using (var document = PdfDocument.Open(path))
            {
                foreach (var page in document.GetPages())
                {
                    var paths = page.ExperimentalAccess.Paths;
                    if (paths.Count < 5)
                    {
                        continue;
                    }

                    var words = PdfHelper.Split(page.GetWords()).ToList();                    

                    var dateHeader = paths[paths.Count - 5].GetBoundingRectangle().Value;
                    var descriptionHeader = paths[paths.Count - 4].GetBoundingRectangle().Value;
                    var amountHeader = paths[paths.Count - 3].GetBoundingRectangle().Value;
                    var footer = paths[paths.Count - 1].GetBoundingRectangle().Value;


                    var dateWords = words.Where(w => w.BoundingBox.Centroid.X > dateHeader.Left &&
                                                     w.BoundingBox.Centroid.X < dateHeader.Right &&
                                                     w.BoundingBox.Centroid.Y < dateHeader.Top)
                                         .ToList();
                    var dateTimes = new List<Tuple<double, DateTime>>();
                    for (var i = 0; i < dateWords.Count; i += 4)
                    {
                        var y = dateWords[i].BoundingBox.Centroid.Y;

                        var dateToken = dateWords[i].Text;
                        var timeToken = dateWords[i + 1].Text;

                        var date = DateTime.ParseExact(dateToken, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        var time = DateTime.ParseExact(timeToken, "HH:mm", CultureInfo.InvariantCulture);

                        var dateTime = date + time.TimeOfDay;
                        dateTimes.Add(Tuple.Create(y, dateTime));
                    }

                    // category is on line with date time
                    // description is between lines wih date time
                    var descriptions = new List<string>();
                    for (var i = 0; i < dateTimes.Count; i++)
                    {
                        var top = dateTimes[i].Item1;
                        double bottom;
                        if (i == dateTimes.Count-1)
                        {
                            bottom = footer.Bottom;
                        }
                        else
                        {
                            bottom = dateTimes[i+1].Item1;
                        }

                        var descriptionWords = words.Where(w => w.BoundingBox.Centroid.X > descriptionHeader.Left &&
                                                                w.BoundingBox.Centroid.X < descriptionHeader.Right &&
                                                                w.BoundingBox.Top < top &&
                                                                w.BoundingBox.Bottom > bottom)
                                                    .ToList();
                        var description = string.Join(" ", descriptionWords.Select(w => w.Text));
                        descriptions.Add(description);
                    }


                    var amountWords = words.Where(w => w.BoundingBox.Centroid.X > amountHeader.Left &&
                                                       w.BoundingBox.Centroid.X < amountHeader.Right &&
                                                       w.BoundingBox.Centroid.Y < amountHeader.Top &&
                                                       w.BoundingBox.Centroid.Y > footer.Top);
                    var amountLines = PdfHelper.GetLinesByBottom(amountWords);
                    var amounts = new List<double>();
                    foreach (var line in amountLines)
                    {
                        var amountToken = line.Replace(',', '.').Replace(" ", "");
                        if (string.IsNullOrEmpty(amountToken))
                        {
                            throw new ParsingException("Empty amount token");
                        }
                        if (!double.TryParse(amountToken, out double amount))
                        {
                            throw new ParsingException($"Could not parse an amount token {amountToken}");
                        }

                        if (amountToken[0] == '+')
                        {
                            amounts.Add(amount);
                        }
                        else
                        {
                            amounts.Add(-amount);
                        }
                    }

                    var pageOperationsCount = dateTimes.Count;
                    if (descriptions.Count != pageOperationsCount || amounts.Count != pageOperationsCount)
                    {
                        throw new ParsingException("Found different number of attributes");
                    }

                    for (var i = 0; i < pageOperationsCount; i++)
                    {
                        operations.Add(new Operation
                        {
                            Amount = amounts[i],
                            Description = descriptions[i],
                            DateTime = dateTimes[i].Item2
                        });
                    }
                }
            }

            return operations;
        }
    }
}
