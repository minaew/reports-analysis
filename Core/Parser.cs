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
            foreach (var file in GetFilePaths(path))
            {
                foreach (var operation in GetParser(file).Parse(file))
                {
                    if (operation.IsUnknownCategory)
                    {
                        var category = _categories.GetCategory(operation);
                        var newOperation = operation.WithCategory(category);
                        yield return newOperation;
                    }
                    else
                    {
                        yield return operation;
                    }
                }
            }
        }

        public IEnumerable<Operation> Parse(Stream stream)
        {
            foreach (var operation in GetParser(stream).Parse(stream))
            {
                if (operation.IsUnknownCategory)
                {
                    var category = _categories.GetCategory(operation);
                    var newOperation = operation.WithCategory(category);
                    yield return newOperation;
                }
                else
                {
                    yield return operation;
                }
            }
        }

        private IEnumerable<string> GetFilePaths(string path)
        {
            if (new DirectoryInfo(path).Exists)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            if (new FileInfo(path).Exists)
            {
                return new[] { path };
            }
            return Enumerable.Empty<string>();
        }

        private IParser GetParser(string path)
        {
            return FormatDetector.GetFormat(path) switch
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
        }

        private IParser GetParser(Stream stream)
        {
            return FormatDetector.GetFormat(stream) switch
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
        }
    }
}
