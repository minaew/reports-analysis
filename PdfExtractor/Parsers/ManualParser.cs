using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class ManualParser : IParser
    {
        private const string Separator = "-";

        public IReadOnlyList<Operation> Parse(string path)
        {
            return ParseInternal(path).ToList();
        }
        
        private static IEnumerable<Operation> ParseInternal(string path)
        {
            using var file = File.OpenText(path);
            while (true)
            {
                var line = file.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    yield break;
                }

                var tokens = line.Split(Separator);
                if (tokens.Length != 3)
                {
                    throw new InvalidOperationException();
                }

                yield return new Operation
                {
                    DateTime = DateTime.ParseExact(tokens[0].Trim(), "dd.MM.yyyy", null),
                    Amount = Money.FromString(tokens[1].Trim()),
                    Description = tokens[2].Trim()
                };
            }
        }
        
        /*
        public IReadOnlyList<Operation> Parse(string path)
        {
            var operations = new List<Operation>();

            var content = File.ReadAllLines(path); // iterations ?
            var account = content[0];
            for (var i = 1; i < content.Length; i++)
            {
                var line = content[i];
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                if (!TryParse(line, "dd.MM.yyyy HH:mm", out Operation operation))
                {
                    if (!TryParse(line, "dd.MM.yyyy", out operation))
                    {
                        throw new ParsingException($"Could not parse \'{line}\'");
                    }
                }
                
                operation.Account = account;
                operations.Add(operation);
            }

            return operations;            
        }

        private static bool TryParse(string line, string datetimeFormat, out Operation operation)
        {
            if (line.Length < datetimeFormat.Length)
            {
                operation = new Operation();
                return false;
            }


            if (!DateTime.TryParseExact(line[..datetimeFormat.Length], datetimeFormat, null, DateTimeStyles.None, out DateTime dateTime))
            {
                operation = new Operation();
                return false;
            }

            var amountToken = line[(datetimeFormat.Length + 1)..].Split(' ')[0].Replace(',', '.');
            operation = new Operation
            {
                DateTime = dateTime,
                Amount = double.Parse(amountToken, NumberStyles.Float, CultureInfo.InvariantCulture),
                Description = line[(datetimeFormat.Length + amountToken.Length + 2)..]
            };
            return true;
        }
        */
    }
}
