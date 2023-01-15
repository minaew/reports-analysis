using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UglyToad.PdfPig;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class TinkoffParser : IParser
    {
        private const int HeaderHeight = 16;
        private const string DateTimePattern = "dd.MM.yy HH:mm";
        private const string DatePattern = "dd.MM.yy";

        public IEnumerable<Operation> Parse(string path)
        {
            var operation = new Operation();
            foreach (var line in GetContent(path).Skip(HeaderHeight))
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

        private static IReadOnlyCollection<string> GetContent(string path)
        {
            using var document = PdfDocument.Open(path);

            return document.GetPages()
                .Select(page => page.GetWords())
                .Select(words => PdfHelper.GetLines(words)
                                          .Select(w => w.Item2))
                .SelectMany(lines => lines)
                .ToList();
        }

        private static Operation ParseAfterDate(string line)
        {
            var tokens = line.Split('₽') // todo: another currency
                             .Select(t => t.Trim())
                             .ToList();

            var amount = double.Parse(tokens[0].Replace(" ", ""));
            if (!tokens[0].StartsWith("+"))
            {
                amount = -amount;
            }
            return new Operation
            {
                Amount = new Money(amount, "rub"), // todo: another currency
                Description = tokens[2]
            };
        }
    }
}
