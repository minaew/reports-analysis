using System;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class InvalidParser : IRanger
    {
        public DateRange GetRange(string path) => new DateRange(DateTime.MinValue, DateTime.MinValue);
    }
}
