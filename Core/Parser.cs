using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Parsers;

namespace ReportAnalysis.Core
{
    public class Parser : IParser
    {
        private readonly ICategories _categories = new EmptyCategories();

        public Parser()
        {
        }

        public Parser(ICategories categories)
        {
            _categories = categories;
        }

        public IEnumerable<Operation> Parse(string path)
        {
            if (new DirectoryInfo(path).Exists)
            {
                foreach (var operation in Directory.GetFileSystemEntries(path).SelectMany(Parse))
                {
                    yield return operation;
                }

                yield break;
            }

            IParser parser = FormatDetector.GetFormat(path) switch
            {
                Format.Sber => new SberParser(),
                Format.SberVklad => new SberVkladParser(),
                Format.Tinkoff => new TinkoffParser(),
                Format.RawText => new ManualParser(),
                Format.Deniz => new DenizParser(),
                Format.ExpencesApp => new ExpensesAppParser(),
                Format.Ararat => new AraratParser(),
                Format.Ziraat => new ZiraatParser(),
                _ => new StubParser(),
            };

            foreach (var operation in parser.Parse(path))
            {
                var newOperation = operation;
                if (string.IsNullOrEmpty(newOperation.Category))
                {
                    newOperation.Category = _categories.GetCategory(operation);
                }

                yield return newOperation;
            }
        }
    }
}
