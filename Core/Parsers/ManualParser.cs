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
            using var file = File.OpenText(path);
            return GetRange(file.BaseStream);
        }

        public DateRange GetRange(Stream stream)
        {
            var dates = Parse(stream).Select(o => o.DateTime).ToList();
            return new DateRange(dates.Min(), dates.Max());
        }

        public IEnumerable<Operation> Parse(string path)
        {
            var account = Path.GetFileNameWithoutExtension(path);

            using var file = File.OpenText(path);
            foreach (var operation in Parse(file.BaseStream))
            {
                var newOperation = operation;
                newOperation.Account = account;
                yield return newOperation;
            }
        }

        public IEnumerable<Operation> Parse(Stream stream)
        {
            var reader = new StreamReader(stream);

            var header = reader.ReadLine();
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
                var line = reader.ReadLine();
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
                    Category = category
                };
            }
        }
    }
}
