using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using ReportAnalysis.Core;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var categorizeCommand = new Command("categorize", "movements categorization");
            var movementsOption = new Option<string>(name: "--movements", "the path of file or directory with files");
            categorizeCommand.AddOption(movementsOption);
            var categoriesOption = new Option<string>(name: "--categories", () => string.Empty);
            categorizeCommand.AddOption(categoriesOption);
            var casesOption = new Option<string>(name: "--cases", () => string.Empty);
            categorizeCommand.AddOption(casesOption);
            categorizeCommand.SetHandler(Categorize, movementsOption, categoriesOption, casesOption);

            var identifyCommand = new Command("identify", "movements owner identification");
            identifyCommand.AddOption(movementsOption);
            identifyCommand.SetHandler(Identify, movementsOption);

            var coverageCommand = new Command("coverage", "movements coverage");
            coverageCommand.AddOption(movementsOption);
            coverageCommand.SetHandler(CalculateCoverage, movementsOption);

            var summariesCommand = new Command("sum", "categories summaries");
            var operationsPathOption = new Option<string>(name: "--operations-path", () => string.Empty);
            summariesCommand.AddOption(operationsPathOption);
            summariesCommand.SetHandler(GetSummaries, operationsPathOption);

            var filterCommand = new Command("filter", "filter operations");
            filterCommand.AddOption(operationsPathOption);
            var categoryOption = new Option<string>(name: "--category", () => string.Empty);
            filterCommand.AddOption(categoryOption);
            filterCommand.SetHandler(Filter, operationsPathOption, categoryOption);

            var root = new RootCommand("some financial parsers and analyzers");
            root.AddCommand(categorizeCommand);
            root.AddCommand(identifyCommand);
            root.AddCommand(coverageCommand);
            root.AddCommand(summariesCommand);
            root.AddCommand(filterCommand);

            return root.Invoke(args);
        }

        private static void Categorize(string movements, string categoriesPath, string cases)
        {
            if (string.IsNullOrEmpty(movements))
            {
                Console.WriteLine($"{Assembly.GetExecutingAssembly().GetName().Name} path [categories-file] [cases-file]");
                return;
            }

            var operations = new Parser(CreateCategories(categoriesPath, cases)).Parse(movements);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var content = JsonSerializer.Serialize(operations.OrderBy(o => o.DateTime), options);
            Console.WriteLine(content);
        }

        private static ICategories CreateCategories(string categoriesPath, string cases)
        {
            if (string.IsNullOrEmpty(categoriesPath) && string.IsNullOrEmpty(cases))
            {
                return new EmptyCategories();
            }

            if (string.IsNullOrEmpty(cases))
            {
                return new Categories(categoriesPath);
            }

            return new Categories(categoriesPath, cases);
        }

        private static void Identify(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.EnumerateFiles(path))
                {
                    Identify(file);
                }
            }
            else
            {
                var id = new Identifier().Identify(path);
                Console.WriteLine($"{path} -> {id}");
            }
        }

        private static void CalculateCoverage(string path)
        {
            IEnumerable<string> files;
            if (Directory.Exists(path))
            {
                files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList();
            }
            else
            {
                files = new[] { path };
            }

            var ranges = new Dictionary<string, DateRange>();
            foreach (var file in files)
            {
                var id = new Identifier().Identify(file);
                var range = new Ranger().GetRange(file);
                if (ranges.ContainsKey(id))
                {
                    var oldRange = ranges[id];
                    ranges[id] = oldRange.Add(range);
                }
                else
                {
                    ranges[id] = range;
                }
            }

            foreach (var entry in ranges)
            {
                Console.WriteLine($"{entry.Key},{entry.Value.From.ToString("dd.MM.yyyy")},{entry.Value.To.ToString("dd.MM.yyyy")}");
            }
        }

        private static void GetSummaries(string operationsPath)
        {
            var content = File.ReadAllText(operationsPath);
            var operations = JsonSerializer.Deserialize<IEnumerable<Operation>>(content) ?? throw new ParsingException();
            foreach (var period in operations.Where(o => o.Amount.Value < 0)
                                             .Where(o => o.Category != "transfer")
                                             .GroupBy(o => o.DateTime.ToString("MM.yyyy")))
            {
                var grouped = string.Join(",", period.GroupBy(o => o.Category).Select(cat =>
                {
                    var total = cat.Select(o => o.Amount).Select(a => new AggregatedMoney(a)).Select(am => am.TotalRub).Sum();
                    return $"{cat.Key}={total}";
                }));
                Console.WriteLine($"{period.Key}:{grouped}");
            }
            // foreach (var period in operations.Where(o => o.Category != "transfer")
            //                                  .GroupBy(o => o.DateTime.ToString("MM.yyyy")))
            // {
            //     var balance = period.Select(o => o.Amount).Select(a => new AggregatedMoney(a)).Select(am => am.TotalRub).Sum();
            //     var balanceStr = ((int)balance).ToString().PadLeft(10, ' ');

            //     var income = period.Select(o => o.Amount).Select(a => new AggregatedMoney(a)).Select(am => am.TotalRub).Where(t => t > 0).Sum();
            //     var incomeStr = ((int)income).ToString().PadLeft(10, ' ');

            //     var outcome = period.Select(o => o.Amount).Select(a => new AggregatedMoney(a)).Select(am => am.TotalRub).Where(t => t < 0).Sum();
            //     var outcomeStr = ((int)outcome).ToString().PadLeft(10, ' ');

            //     var salary = period.Where(o => o.Category == "salary").Select(o => o.Amount).Select(a => new AggregatedMoney(a)).Select(am => am.TotalRub).Where(t => t < 0).Sum();
            //     var salaryStr = ((int)outcome).ToString().PadLeft(10, ' ');

            //     Console.WriteLine($"{period.Key}:balance={balanceStr}, income={incomeStr}, outcome={outcomeStr}, salary={salaryStr}");
            // }
        }

        private static void Filter(string operationsPath, string categoryFilter)
        {
            var content = File.ReadAllText(operationsPath);
            var operations = JsonSerializer.Deserialize<IEnumerable<Operation>>(content) ?? throw new ParsingException();
            foreach (var operation in operations.Where(o => o.Category.Contains(categoryFilter)))
            {
                Console.WriteLine($"{operation.DateTime.Date} {operation.Amount.Value} {operation.Amount.Currency} {operation.Description}");
            }
        }
    }
}
