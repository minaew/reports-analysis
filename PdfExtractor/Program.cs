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
            string path = null;

            var operations = Directory.EnumerateFiles(path).SelectMany(Parse).ToList();
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
    }
}
