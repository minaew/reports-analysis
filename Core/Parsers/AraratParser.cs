﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;

namespace ReportAnalysis.Core.Parsers
{
    class AraratParser : IParser, IIdentifier, IRanger
    {
        private readonly Regex _dateRegex = new Regex(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.Compiled);

        public DateRange GetRange(string path)
        {
            var rangeString = PdfHelper.GetContent(path)[6];
            var match = Regex.Match(rangeString, @"\d{2}.\d{2}.\d{4} 00:00-\d{2}.\d{2}.\d{4} 00:00");
            var from = match.Value.Substring(0, 10);
            var to = match.Value.Substring(17, 10);
            return new DateRange(DateTime.ParseExact(from, "dd.MM.yyyy", null),
                                 DateTime.ParseExact(to, "dd.MM.yyyy", null));
        }

        public DateRange GetRange(Stream stream)
        {
            throw new NotImplementedException();
        }

        public string Identify(string path)
        {
            using var document = PdfDocument.Open(path);

            var firstPageWords = document.GetPage(1).GetWords();
            var firstPageLines = PdfHelper.GetLines(firstPageWords).Take(5).ToList();

            var firstPageLine = firstPageLines[0].Item2; // ԱՐԱՐԱՏԲԱՆԿ ԲԲԸ Հաշվի համար` 1510 0588 2510 0100
            var accountNumber = firstPageLine.Substring("ԱՐԱՐԱՏԲԱՆԿ ԲԲԸ Հաշվի համար`".Length + 1).Replace(" ", "");
            firstPageLine = firstPageLines[4].Item2; // MINAEV ALEKSEI- քարտ
            var name = firstPageLine.Substring(0, firstPageLine.Length - "- քարտ".Length);

            return $"ararat {accountNumber} {name}";
        }

        public IEnumerable<Operation> Parse(string path)
        {
            var account = Identify(path);

            using var document = PdfDocument.Open(path);

            var incomeColumnHeaderCenter = document.GetPage(1).GetWords().First(w => w.Text == "Մուտք").BoundingBox.Centroid.X;

            foreach (var page in document.GetPages())
            {
                var words = page.GetWords().ToList();
                var dates = words.Where(w => _dateRegex.IsMatch(w.Text))
                    .Skip(page.Number == 1 ? 3 : 0)
                    .ToList();
                foreach (var date in dates)
                {
                    var line = words.Where(w =>
                            w.BoundingBox.Centroid.Y > date.BoundingBox.Bottom &&
                            w.BoundingBox.Centroid.Y < date.BoundingBox.Top)
                        .ToList();
                    // todo: combine
                    line.RemoveAll(w => w.BoundingBox.Left > 134 && w.BoundingBox.Left < 135);
                    line.RemoveAll(w => w.BoundingBox.Left > 98 && w.BoundingBox.Left < 143);

                    var dateTime = DateTime.ParseExact($"{line[0].Text} {line[1].Text}",
                        "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    var currency = line[2].Text;

                    double value;
                    if (line[3].BoundingBox.Left < incomeColumnHeaderCenter && line[3].BoundingBox.Right > incomeColumnHeaderCenter)
                    {
                        value = double.Parse(line[3].Text, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = -double.Parse(line[3].Text, CultureInfo.InvariantCulture);
                    }

                    var description = string.Join(" ",
                        line.Skip(5).Select(w => w.Text));

                    yield return new Operation
                    {
                        Account = account,
                        Amount = new Money(value, currency),
                        Description = description,
                        DateTime = dateTime
                    };
                }
            }
        }

        public IEnumerable<Operation> Parse(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
