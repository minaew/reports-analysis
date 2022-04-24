using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace PdfExtractor
{
    public class TinkoffParser : IParser
    {
        private readonly Regex _dateTimeRegex = new(@"\d{2}\.\d{2}\.\d{2} \d{2}:\d{2}", RegexOptions.Compiled);
        private readonly Regex _dateRegex = new(@"\d{2}\.\d{2}\.\d{2}", RegexOptions.Compiled);

        private const double AmountLeft = 205;
        private const double AmountRight = 270;
        private const double DescriptionLeft = 300;

        public IReadOnlyList<Operation> Parse(string path)
        {
            var operations = new List<Operation>();

            using (var document = PdfDocument.Open(path))
            {
                foreach (var page in document.GetPages())
                {
                    var words = page.GetWords().ToList();
                    var dateTimeTitle = PdfHelper.GetRect(words, "Дата", "и", "время");
                    if (dateTimeTitle == null) continue;

                    var dates = words.Where(w => w.BoundingBox.Top < dateTimeTitle.Value.Top &&
                                                 w.BoundingBox.Left >= dateTimeTitle.Value.Left &&
                                                 w.BoundingBox.Right <= dateTimeTitle.Value.Right)
                                     .ToList();

                    var dateStrs = PdfHelper.GetLines(dates).ToList();


                    foreach (var d in dateStrs)
                    {
                        var token = d.Item2;
                        DateTime dateTime;
                        if (_dateTimeRegex.IsMatch(d.Item2))
                        {
                            dateTime = DateTime.ParseExact(token, "dd.MM.yy HH:mm", null);
                        }
                        else
                        {
                            if (_dateRegex.IsMatch(d.Item2))
                            {
                                dateTime = DateTime.ParseExact(token, "dd.MM.yy", null);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        var same = words.Where(o => o.BoundingBox.Top >= d.Item1 &&
                                                    o.BoundingBox.Bottom <= d.Item1)
                                        .ToList();

                        var amountWords = same.Where(o => o.BoundingBox.Left > AmountLeft &&
                                                          o.BoundingBox.Right < AmountRight)
                                              .ToList();
                        var amountToken = string.Join("", amountWords.Select(o => o.Text));
                        if (amountToken.StartsWith('+'))
                        {
                            amountToken = amountToken[1..];
                        }
                        else
                        {
                            amountToken = "-" + amountToken;
                        }

                        var descriptionWords = same.Where(o => o.BoundingBox.Left >= DescriptionLeft);
                        var description = string.Join(" ", descriptionWords.Select(d => d.Text));

                        operations.Add(new Operation
                        {
                            DateTime = dateTime,
                            Amount = double.Parse(amountToken, NumberStyles.Float, CultureInfo.InvariantCulture),
                            Description = description
                        });
                    }
                }
            }

            return operations;
        }

        /*
        private IEnumerable<string> GetRows(string documentPath)
        {
            using (var document = PdfDocument.Open(documentPath))
            {
                for (var p = 0; p < document.NumberOfPages - 1; p++)
                {
                    var page = document.GetPage(p+1);
                    // Console.WriteLine("NEW PAGE");
                    string pageText = page.Text;
                    
                    var index = pageText.IndexOf(Header);
                    if (index == -1)
                    {
                        throw new ParsingException();
                    }

                    var tableStartIndex = index + Header.Length;
                    pageText = pageText[tableStartIndex..];

                    var matches = _dateRegex.Matches(pageText);
                    for (var i = 0; i < matches.Count-2; i+=2)
                    {
                        yield return pageText[matches[i].Index..matches[i + 2].Index];
                    }
                    yield return pageText[matches[^2].Index..];
                }
            }
        }
    
        private Operation ParseRow(string row)
        {
            Console.WriteLine(row);
            var m = _dateRegex.Matches(row)[1];

            // two formats are supported
            if (!DateTime.TryParseExact(row[..15], "dd.MM.yy  HH:mm", null, DateTimeStyles.None, out DateTime dateTime))
            {
                dateTime = DateTime.ParseExact(row[..8], "dd.MM.yy", null);
            }

            var index = row.LastIndexOf('₽');
            var index2 = row.LastIndexOfAny(new [] {'₽', '$', '£'}, index-1); // на самом деле нужно искать не пробел, цифру, точку
            var amountToken = row.Substring(index2 + 1, index - index2 - 1).Replace(" ", "");

            var description = row[(index + 1)..];

            return new Operation
            {
                DateTime = dateTime,
                Amount = amountToken[0] == '+' ? double.Parse(amountToken, CultureInfo.InvariantCulture) : -double.Parse(amountToken, CultureInfo.InvariantCulture),
                Description = description
            };
        }
        */
    }
}
