using PdfExtractor.Models;
using PdfExtractor.Parsers;

namespace PdfExtractor
{
    public class MetaRanger : IRanger
    {
        public DateRange GetRange(string path) => GetRanger(path).GetRange(path);

        private IRanger GetRanger(string path) => FormatDetector.GetFormat(path) switch
        {
            Format.Ararat => new AraratParser(),
            _ => new InvalidParser()
        };
    }
}
