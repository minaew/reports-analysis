using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class TinkoffParser : IParser
    {
        private readonly Regex _dateTimeRegex = new(@"\d{2}\.\d{2}\.\d{2} \d{2}:\d{2}", RegexOptions.Compiled);
        private readonly Regex _dateRegex = new(@"\d{2}\.\d{2}\.\d{2}", RegexOptions.Compiled);

        private const double AmountLeft = 205;
        private const double AmountRight = 270;
        private const double DescriptionLeft = 300;

        public IEnumerable<Operation> Parse(string path)
        {
            using var document = PdfDocument.Open(path);
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

                    yield return new Operation
                    {
                        DateTime = dateTime,
                        Amount = new Money
                        {
                            Value = double.Parse(amountToken, NumberStyles.Float, CultureInfo.InvariantCulture)
                        },
                        Description = description
                    };
                }
            }
        }
    }
}
