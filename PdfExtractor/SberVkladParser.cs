using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace PdfExtractor
{
    internal struct OpenRect
    {
        public double Left { get; set; }

        public double Top { get; set; }

        public double Right { get; set; }
    }

    public class SberVkladParser : IParser
    {
        private readonly Regex _dateRegex = new(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.Compiled);

        public IReadOnlyList<Operation> Parse(string path)
        {
            var operations = new List<Operation>();

            using (var document = PdfDocument.Open(path))
            {
                foreach (var page in document.GetPages())
                {
                    var words = page.GetWords().ToList();

                    var dateRect = PdfHelper.GetRect(words, "ДАТА", "ОПЕРАЦИИ");
                    var descriptionRect = PdfHelper.GetRect(words, "НАИМЕНОВАНИЕ", "ОПЕРАЦИИ");
                    var amountRect = PdfHelper.GetRect(words, "СУММА", "ОПЕРАЦИИ");
                    if (!dateRect.HasValue || !descriptionRect.HasValue || !amountRect.HasValue)
                    {
                        continue;
                    }

                    var dateHeader = dateRect.Value;
                    var descriptionHeader = descriptionRect.Value;
                    var amountHeader = amountRect.Value;
                    amountHeader.Right += 2; // СУММА ОПЕРАЦИИ ^3

                    var dates = words.Where(w => w.BoundingBox.Top < dateHeader.Top &&
                                                 w.BoundingBox.Left >= dateHeader.Left &&
                                                 w.BoundingBox.Right <= dateHeader.Right)
                                     .ToList();

                    foreach (var date in dates)
                    {
                        var dateToken = date.Text;
                        if (!_dateRegex.IsMatch(dateToken)) continue;

                        var descriptionWords = words.Where(w => w.BoundingBox.Left >= descriptionHeader.Left &&
                                                                w.BoundingBox.Right <= descriptionHeader.Right &&
                                                                w.BoundingBox.Top >= date.BoundingBox.Centroid.Y &&
                                                                w.BoundingBox.Bottom <= date.BoundingBox.Centroid.Y)
                                                    .ToList();
                        var description = string.Join(" ", descriptionWords.Select(w => w.Text)); // todo: order?

                        var amountWords = words.Where(w => w.BoundingBox.Left >= amountHeader.Left &&
                                                           w.BoundingBox.Right <= amountHeader.Right &&
                                                           w.BoundingBox.Top >= date.BoundingBox.Centroid.Y &&
                                                           w.BoundingBox.Bottom <= date.BoundingBox.Centroid.Y)
                                                .ToList();
                        var amountToken = string.Join("", amountWords.Select(w => w.Text)); // todo: order?
                        amountToken = amountToken.Replace("+", "").Replace(',', '.');

                        if (string.IsNullOrEmpty(description) && string.IsNullOrEmpty(amountToken))
                        {
                            continue;
                        }

                        operations.Add(new Operation
                        {
                            DateTime = DateTime.ParseExact(dateToken, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                            Amount = double.Parse(amountToken, CultureInfo.InvariantCulture),
                            Description = description
                        });
                    }
                }
            }

            return operations;
        }
    }
}
