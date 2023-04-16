using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class InvalidParser : IRanger
    {
        public DateRange GetRange(string path) => default(DateRange);
    }
}
