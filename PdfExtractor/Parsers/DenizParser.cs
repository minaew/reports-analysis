using System.Collections.Generic;
using System.Linq;
using PdfExtractor.Helpers;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class DenizParser : PythonParser
    {
        public DenizParser() : base("deniz_parser.py")
        {
        }

        public override IEnumerable<Operation> Parse(string path)
        {
            var name = ExcelHelper.GetString(path, 1, 1);
            var branch = ExcelHelper.GetString(path, 2, 1);
            var iban = ExcelHelper.GetString(path, 3, 1);
            var account = iban;

            return base.Parse(path).Select(o => o.WithAccount(account));
        }
    }
}
