using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            switch (args[0])
            {
                case "top":
                    string path = args[0];
                    string categoriesFile = args[1];
                    var categories = new Categories(categoriesFile);
                    Top(categories, path);
                    break;
                case "monthly":
                    break;
            }
        }

        private static IReadOnlyCollection<Operation> Parse(string path)
        {
            IParser parser = new FormatDetector().GetFormat(path) switch
            {
                Format.Sber => new SberParser(),
                Format.SberVklad => new SberVkladParser(),
                Format.Tinkoff => new TinkoffParser(),
                Format.RawText => new ManualParser(),
                _ => throw new ArgumentException("unknown file format"),
            };

            return parser.Parse(path);
        }

        private static void Top(Categories categories, string path)
        {
            var operations = new List<Operation>();

            foreach (var file in Directory.EnumerateFiles(path))
            {
                foreach (var operation in Parse(file))
                {
                    var newOperation = operation;

                    newOperation.Account = Path.GetFileName(file);
                    newOperation.Category = categories.GetCategory(operation);

                    operations.Add(newOperation);
                }
            }

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

        private static void Monthly()
        {
        }
    }
}
