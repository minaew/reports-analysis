using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class AraratParser : IParser
    {
        private readonly Regex _dateRegex = new Regex(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.Compiled);
        private const string Account = "ararat";
        
        public IEnumerable<Operation> Parse(string path)
        {
            using var document = PdfDocument.Open(path);
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
                    
                    var dateTime = DateTime.ParseExact($"{line[0].Text} {line[1].Text}",
                        "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                    var currency = line[2].Text;

                    double value;
                    if (line[3].BoundingBox.Left > 214 && line[3].BoundingBox.Left < 215)
                    {
                        value = double.Parse(line[3].Text);
                    }
                    else
                    {
                        value = -double.Parse(line[3].Text);
                    }

                    var description = string.Join(" ", 
                        line.Skip(5).Select(w => w.Text));

                    yield return new Operation
                    {
                        Account = Account,
                        Amount = new Money(value, currency),
                        Description = description,
                        DateTime = dateTime
                    };
                }
            }
        }
    }
}
