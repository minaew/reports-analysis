using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;

namespace ReportAnalysis.Core.Parsers
{
    class SberParser : IParser, IIdentifier, IRanger
    {
        public DateRange GetRange(string path)
        {
            var rangeString = PdfHelper.GetContent(path).Single(l => l.StartsWith("Итого"));
            var v = Regex.Match(rangeString, @"с \d{2}.\d{2}.\d{4} по \d{2}.\d{2}.\d{4}").Value;
            return new DateRange(DateTime.ParseExact(v.Substring(2, 10), "dd.MM.yyyy", null),
                                 DateTime.ParseExact(v.Substring(16, 10), "dd.MM.yyyy", null));
        }

        public string Identify(string path)
        {
            var client = PdfHelper.GetContent(path)[4];
            var identityWords = PdfHelper.GetWords(path)
                                         .SkipWhile(w => w.BoundingBox.Bottom > 662.86)
                                         .TakeWhile(w => w.BoundingBox.Right < 228);

            return client + " " + PdfHelper.Join(identityWords);
        }

        public IEnumerable<Operation> Parse(string path)
        {
            using var document = PdfDocument.Open(path);
            foreach (var page in document.GetPages())
            {
                var dateRect = PdfHelper.GetHeaderRectangle(page, "ДАТА ОПЕРАЦИИ (МСК)");
                var descriptionRect = PdfHelper.GetHeaderRectangle(page, "КАТЕГОРИЯ");
                var amountRect = PdfHelper.GetHeaderRectangle(page, "СУММА В ВАЛЮТЕ СЧЁТА");
                if (!dateRect.HasValue || !descriptionRect.HasValue || !amountRect.HasValue)
                {
                    continue;
                }
                var dateHeader = dateRect.Value;
                var descriptionHeader = descriptionRect.Value;
                var amountHeader = amountRect.Value;

                var words = PdfHelper.Split(page.GetWords()).ToList();
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

                    if (!DateTime.TryParseExact(dateToken, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out DateTime date))
                    {
                        break;
                    }
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
                    double bottom = 0;
                    if (i < dateTimes.Count - 1)
                    {
                        bottom = dateTimes[i + 1].Item1;
                    }

                    var descriptionWords = words.Where(w => w.BoundingBox.Centroid.X > descriptionHeader.Left &&
                                                            w.BoundingBox.Centroid.X < descriptionHeader.Right &&
                                                            w.BoundingBox.Top < top &&
                                                            w.BoundingBox.Bottom > bottom)
                        .ToList();
                    var description = string.Join(" ", descriptionWords.Select(w => w.Text));

                    foreach (var suffix in new[] { " Продолжение на следующей", " на счёт кредитной карты" })
                    {
                        var index = description.IndexOf(suffix);
                        if (index != -1)
                        {
                            description = description[..index];
                        }
                    }

                    descriptions.Add(description);
                }

                var amountWords = words.Where(w => w.BoundingBox.Centroid.X > amountHeader.Left &&
                                                   w.BoundingBox.Centroid.X < amountHeader.Right &&
                                                   w.BoundingBox.Centroid.Y < amountHeader.Top);
                var amountLines = PdfHelper.GetLinesByBottom(amountWords);
                var amounts = new List<double>();
                foreach (var line in amountLines)
                {
                    var amountToken = line.Replace(',', '.').Replace(" ", "");
                    if (string.IsNullOrEmpty(amountToken))
                    {
                        throw new ParsingException("Empty amount token");
                    }
                    if (!double.TryParse(amountToken, NumberStyles.Float, CultureInfo.InvariantCulture, out double amount))
                    {
                        continue;
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
                    yield return new Operation
                    {
                        Amount = new Money(amounts[i], "rub"),
                        Description = descriptions[i],
                        DateTime = dateTimes[i].Item2
                    };
                }
            }
        }
    }
}
