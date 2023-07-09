using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class ManualParser : IParser, IRanger
    {
        private const string Separator = ",";

        public DateRange GetRange(string path)
        {
            var dates = Parse(path).Select(o => o.DateTime).ToList();
            return new DateRange(dates.Min(), dates.Max());
        }

        public IEnumerable<Operation> Parse(string path)
        {
            using var file = File.OpenText(path);

            var header = file.ReadLine();
            if (string.IsNullOrEmpty(header))
            {
                throw new ParsingException("invalid header");
            }
            var headerTokens = header.Split(",");
            if (headerTokens.Length != 2)
            {
                throw new ParsingException("invalid header");
            }
            var year = int.Parse(headerTokens[0], CultureInfo.InvariantCulture);
            var currency = headerTokens[1];

            while (true)
            {
                var line = file.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    yield break;
                }

                var tokens = line.Split(Separator);
                if (tokens.Length != 4 && tokens.Length != 3)
                {
                    throw new InvalidOperationException();
                }
                var category = string.Empty;
                if (tokens.Length == 4)
                {
                    category = tokens[3].Trim();
                }

                var dateTime = DateTime.ParseExact(tokens[0].Trim(), "dd.MM", null);
                dateTime = new DateTime(year, dateTime.Month, dateTime.Day);

                var amount = Money.FromString(tokens[1].Trim() + " " + currency.Trim());

                yield return new Operation
                {
                    DateTime = dateTime,
                    Amount = amount,
                    Description = tokens[2].Trim(),
                    Category = category,
                    Account = Path.GetFileNameWithoutExtension(path)
                };
            }
        }
    }
}
