using System.Collections.Generic;
using System.Linq;
using PdfExtractor.Helpers;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class ZiraatParser : PythonParser
    {
        public ZiraatParser() : base("ziraat_parser.py")
        {
        }

        public override IEnumerable<Operation> Parse(string path)
        {
            var accountNumber = ExcelHelper.GetString(path, 6, 2);
            var iban = ExcelHelper.GetString(path, 7, 2);
            var account = iban;

            return base.Parse(path).Select(o => o.WithAccount(account));
        }
    }
}
