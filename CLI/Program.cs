using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfExtractor;
using PdfExtractor.Models;
using PdfExtractor.Parsers;

namespace CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var outStream = Console.Out;
            if (args.LastOrDefault()?.StartsWith(">") ?? false)
            {
                outStream = new StreamWriter(args.Last().TrimStart('>'));
                args = args.Take(args.Length - 1).ToArray();
            }
            if (args.Length < 1)
            {
                Console.WriteLine("path [categories-file] [cases-file]");
                return -1;
            }

            var path = args[0];
            var categoriesFile = args.Length > 1 ? args[1] : string.Empty;
            var casesFile = args.Length > 2 ? args[2] : string.Empty;
            ICategories categories;
            if (string.IsNullOrEmpty(categoriesFile) || string.IsNullOrEmpty(casesFile))
            {
                categories = new EmptyCategories();
            }
            else
            {
                categories = new Categories(categoriesFile, casesFile);
            }

            var operations = Parse(path, categories);

            var mode = "--list";
            switch (mode)
            {
                case "--top":
                    Top(operations);
                    break;

                case "--monthly":
                    Monthly(operations);
                    break;

                case "--find":
                    var request = args[3].Trim('\"');
                    foreach (var operation in operations.Where(o => o.Description == request))
                    {
                        outStream.WriteLine(operation.Serialize());
                    }
                    break;

                case "--list":
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var content = JsonSerializer.Serialize(operations.OrderBy(o => o.DateTime), options);
                    outStream.WriteLine(content);
                    break;
            }
            
            outStream.Dispose();
            return 0;
        }

        private static void Top(IEnumerable<Operation> operations)
        {
            var dict = operations.Where(o => string.IsNullOrEmpty(o.Category))
                                 .GroupBy(o => o.Description)
                                 .ToDictionary(g => g.Key, g => g.Count())
                                 .OrderByDescending(p => p.Value)
                                 .ToList();

            for (var i = 0; i < 20; i++)
            {
                var pair = dict[i];
                Console.WriteLine($"{pair.Value}\t{pair.Key}");
            }
        }

        private static void Monthly(IEnumerable<Operation> operations)
        {
            foreach (var month in operations.GroupBy(o => Tuple.Create(o.DateTime.Year, o.DateTime.Month))
                                            .OrderBy(p => p.Key))
            {
                var outcomeOperations = month.Where(o => o.Category != "internal").Where(o => o.Amount.Value < 0);
                var incomeOperations = month.Where(o => o.Category != "internal").Where(o => o.Amount.Value > 0);

                var outcome = outcomeOperations.Select(m => m.Amount.Value).Sum();
                var income = incomeOperations.Select(m => m.Amount.Value).Sum();

                Console.WriteLine($"{month.Key}\t{income}\t{outcome}");
            }
        }

        private static IEnumerable<Operation> Parse(string path, ICategories categories)
        {
            if (new DirectoryInfo(path).Exists)
            {
                foreach (var operation in Directory.GetFileSystemEntries(path).SelectMany(p => Parse(p, categories)))
                {
                    yield return operation;
                }

                yield break;
            }

            IParser parser = new FormatDetector().GetFormat(path) switch
            {
                Format.Sber => new SberParser(),
                Format.SberVklad => new SberVkladParser(),
                Format.Tinkoff => new TinkoffParser(),
                Format.RawText => new ManualParser(),
                Format.Deniz => new DenizParser(),
                Format.ExpencesApp => new ExpensesAppParser(),
                _ => new StubParser(),
            };

            foreach (var operation in parser.Parse(path))
            {
                var newOperation = operation;

                newOperation.Account = Path.GetFileName(path);
                newOperation.Category = categories.GetCategory(operation);

                yield return newOperation;
            }
        }
    }
}
