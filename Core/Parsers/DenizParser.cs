﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReportAnalysis.Core.Helpers;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class DenizParser : PythonParser, IIdentifier, IRanger
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
            var client = ExcelHelper.GetStringOrThrow(path, 1, 1);
            var iban = ExcelHelper.GetStringOrThrow(path, 3, 1);
            return $"deniz {client} {iban}";
        }

        public DateRange GetRange(string path)
        {
            var rangeString = ExcelHelper.GetStringOrThrow(path, 4, 1);
            return DateRange.Parse(rangeString.Replace(" ", ""));
        }

        public DateRange GetRange(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
