using System.Collections.Generic;
using System.Linq;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class StubParser : IParser, IRanger
    {
        public IEnumerable<Operation> Parse(string path) => Enumerable.Empty<Operation>().ToList();

        public DateRange GetRange(string path) => default(DateRange);
    }
}
