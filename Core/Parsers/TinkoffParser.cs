using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;

namespace ReportAnalysis.Core.Parsers
{
    class TinkoffParser : IParser, IIdentifier, IRanger
    {
        private const int HeaderHeight = 16;
        private const string DateTimePattern = "dd.MM.yy HH:mm";
        private const string DatePattern = "dd.MM.yy";

        public string Identify(string path)
        {
            var content = PdfHelper.GetContent(path);
            return content[0] + " " + content[5].Split(' ').Last();
        }

        public IEnumerable<Operation> Parse(string path)
        {
            var operation = new Operation();
            foreach (var line in PdfHelper.GetContent(path).Skip(HeaderHeight))
            {
                if (line.StartsWith("Операции по карте") ||
                    line.StartsWith("Дата и время") ||
                    line.StartsWith("операции банком"))
                {
                    continue;
                }

                if (line.StartsWith("Адреса банкоматов"))
                {
                    yield return operation;
                }

                DateTime dateTime;
                if (line.Length >= DateTimePattern.Length &&
                    DateTime.TryParseExact(line.Substring(0, DateTimePattern.Length), DateTimePattern, null, DateTimeStyles.None, out dateTime))
                {
                    if (!operation.Equals(default(Operation)))
                    {
                        yield return operation;
                    }

                    operation = ParseAfterDate(new string(line.Skip(DateTimePattern.Length + 9).ToArray()));
                    operation.DateTime = dateTime;
                }
                else if (line.Length >= DatePattern.Length &&
                    DateTime.TryParseExact(line.Substring(0, DatePattern.Length), DatePattern, null, DateTimeStyles.None, out dateTime))
                {
                    if (!operation.Equals(default(Operation)))
                    {
                        yield return operation;
                    }

                    operation = ParseAfterDate(new string(line.Skip(DatePattern.Length + 9).ToArray()));
                    operation.DateTime = dateTime;
                }
                else
                {
                    operation.Description += " " + line;
                }
            }
        }

        private static Operation ParseAfterDate(string line)
        {
            var border = line.LastIndexOf('₽');

            var amountToken = new string(line.Substring(0, border).Replace(" ", "")
                .Reverse().TakeWhile(c => char.IsDigit(c) || c == '.' || c == '+').Reverse()
                .ToArray());

            var amount = double.Parse(amountToken);
            if (!amountToken.StartsWith("+"))
            {
                amount = -amount;
            }
            return new Operation
            {
                Amount = new Money(amount, "rub"),
                Description = line.Substring(border + 1, line.Length - border - 1).Trim()
            };
        }

        public DateRange GetRange(string path)
        {
            var rangeString = PdfHelper.GetContent(path).Where(l => l.StartsWith("Баланс")).Take(2).ToList();
            var fromString = rangeString[0];
            var from = Regex.Match(fromString, @"\d{2}.\d{2}.\d{4}");
            if (string.IsNullOrEmpty(from.Value))
            {
                throw new ParsingException($"string {fromString} does not contain date");
            }
            var toString = rangeString[1];
            var to = Regex.Match(toString, @"\d{2}.\d{2}.\d{4}");
            if (string.IsNullOrEmpty(to.Value))
            {
                throw new ParsingException($"string {toString} does not contain date");
            }

            return new DateRange(DateTime.ParseExact(from.Value, "dd.MM.yyyy", null),
                                 DateTime.ParseExact(to.Value, "dd.MM.yyyy", null));
        }
    }
}
