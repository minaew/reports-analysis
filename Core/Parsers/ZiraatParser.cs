﻿using System.Collections.Generic;
using System.IO;
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

        public string Identify(string path)
        {
            var iban = ExcelHelper.GetStringOrThrow(path, 7, 2);
            var client = ExcelHelper.GetStringOrThrow(path, 4, 0);
            return $"ziraat {client.Substring(5)} {iban}";
        }

        public DateRange GetRange(string path)
        {
            var rangeString = ExcelHelper.GetStringOrThrow(path, 5, 0);
            var v = Regex.Match(rangeString, @"\d{2}.\d{2}.\d{4} - \d{2}.\d{2}.\d{4}").Value;
            return DateRange.Parse(v.Replace(" ", ""));
        }

        public DateRange GetRange(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
