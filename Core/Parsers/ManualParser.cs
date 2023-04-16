using System;
using System.Collections.Generic;
using System.IO;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class ManualParser : IParser
    {
        private const string Separator = ",";

        public IEnumerable<Operation> Parse(string path)
        {
            using var file = File.OpenText(path);
            var line = file.ReadLine(); // todo: range

            while (true)
            {
                line = file.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    yield break;
                }

                var tokens = line.Split(Separator);
                if (tokens.Length != 4)
                {
                    throw new InvalidOperationException();
                }

                yield return new Operation
                {
                    DateTime = DateTime.ParseExact(tokens[0].Trim(), "dd.MM.yyyy", null),
                    Amount = Money.FromString(tokens[1].Trim()),
                    Description = tokens[2].Trim(),
                    Category = tokens[3].Trim(),
                    Account = Path.GetFileNameWithoutExtension(path)
                };
            }
        }
    }
}
