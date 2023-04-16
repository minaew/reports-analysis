using System.Collections.Generic;
using System.Linq;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class ZiraatParser : PythonParser, IIdentifier
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
    }
}
