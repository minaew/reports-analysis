using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class ZiraatParser : PythonParser, IIdentifier, IRanger
    {
        public ZiraatParser() : base("ziraat_parser.py")
        {
        }

        public override IEnumerable<Operation> Parse(string path)
        {
            var accountNumber = ExcelHelper.GetString(path, 6, 2);
            var account = Identify(path);

            return base.Parse(path).Select(o => o.WithAccount(account));
        }

        public string Identify(string path) => ExcelHelper.GetStringOrThrow(path, 7, 2);

        public DateRange GetRange(string path)
        {
            var rangeString = ExcelHelper.GetStringOrThrow(path, 5, 0);
            var v = Regex.Match(rangeString, @"\d{2}.\d{2}.\d{4} - \d{2}.\d{2}.\d{4}").Value;
            return DateRange.Parse(v.Replace(" ", ""));
        }
    }
}
