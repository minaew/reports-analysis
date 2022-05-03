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
        static void Main(string[] args)
        {
            string path = args[0];
            string categoriesFile = args[1];
            var categories = new Categories(categoriesFile);
            var operations = Parse(path, categories);

            switch (args[2])
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
                        Console.WriteLine(operation.Serialize());
                    }
                    break;

                case "--tree":
                    Tree(operations);
                    break;

                case "--list":
                    Console.WriteLine(JsonSerializer.Serialize(operations, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
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
                var outcomeOperations = month.Where(o => o.Category != "internal").Where(o => o.Amount < 0);
                var incomeOperations = month.Where(o => o.Category != "internal").Where(o => o.Amount > 0);
                
                var outcome = outcomeOperations.Select(m => m.Amount).Sum();
                var income = incomeOperations.Select(m => m.Amount).Sum();

                Console.WriteLine($"{month.Key}\t{income}\t{outcome}");
                // foreach (var outcomeOperation in outcomeOperations)
                // {
                //     Console.WriteLine($"{outcomeOperation.Account}\t{outcomeOperation.Amount}\t{outcomeOperation.Description}\t{outcomeOperation.Category}");
                // }
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
    
        private static void Tree(IEnumerable<Operation> operations)
        {
            var tree = new Root();
            foreach (var operation in operations)
            {
                var yearKey = operation.DateTime.Year;
                if (!tree.Years.ContainsKey(yearKey))
                {
                    tree.Years[yearKey] = new Year();
                }
                var year = tree.Years[yearKey];

                var monthKey = operation.DateTime.Month;
                if (!year.Months.ContainsKey(monthKey))
                {
                    year.Months[monthKey] = new Month();
                }
                var month = year.Months[monthKey];

                var categoryKey = operation.Category;
                if (!month.Categories.ContainsKey(categoryKey))
                {
                    month.Categories[categoryKey] = new Category();
                }
                var category = month.Categories[categoryKey];
                category.Operations.Add(operation);
            }

            Console.WriteLine(JsonSerializer.Serialize(tree, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
