using System;
using System.Collections.Generic;
using System.Linq;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class DenizParser : PythonParser, IIdentifier
    {
        public DenizParser() : base("deniz_parser.py")
        {
        }

        public override IEnumerable<Operation> Parse(string path)
        {
            var name = ExcelHelper.GetString(path, 1, 1);
            var branch = ExcelHelper.GetString(path, 2, 1);
            var account = Identify(path);

            return base.Parse(path).Select(o => o.WithAccount(account));
        }

        public string Identify(string path)
        {
            return ExcelHelper.GetString(path, 3, 1) ?? throw new InvalidOperationException("string is null");
        }
    }
}
