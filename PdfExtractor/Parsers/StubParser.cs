using System.Collections.Generic;
using System.Linq;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class StubParser : IParser
    {
        public IEnumerable<Operation> Parse(string path)
        {
            return Enumerable.Empty<Operation>().ToList();
        }
    }
}
