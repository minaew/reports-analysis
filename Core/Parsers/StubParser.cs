using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class StubParser : IParser, IRanger
    {
        public IEnumerable<Operation> Parse(string path) => Enumerable.Empty<Operation>().ToList();

        public DateRange GetRange(string path) => new DateRange(default(DateTime), default(DateTime));

        public IEnumerable<Operation> Parse(Stream stream)
        {
            throw new NotImplementedException();
        }

        public DateRange GetRange(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
