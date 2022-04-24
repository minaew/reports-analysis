using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfExtractor.Models;
using PdfExtractor.Parsers;

namespace PdfExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            string categoriesFile = args[1];
            var categories = new Categories(categoriesFile);
            var operations = Parse(path, categories);

            switch (args[2])
            {
                case "top":
                    Top(operations);
                    break;

                case "monthly":
                    Monthly(operations);
                    break;
            }
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
                var outcome = month.Where(o => o.Account != "internal").Select(m => m.Amount).Where(a => a < 0).Sum();
                var income = month.Where(o => o.Account != "internal").Select(m => m.Amount).Where(a => a > 0).Sum();

                Console.WriteLine($"{month.Key}\t{income}\t{outcome}");
                foreach (var outcomeOperation in month.Where(o => o.Amount < 0))
                {
                    Console.WriteLine($"{outcomeOperation.Account}\t{outcomeOperation.Amount}\t{outcomeOperation.Description}\t{outcomeOperation.Category}");
                }
            }
        }

        private static IEnumerable<Operation> Parse(string path, Categories categories)
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
                _ => throw new ArgumentException("unknown file format"),
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
