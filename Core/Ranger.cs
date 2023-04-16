using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;
using ReportAnalysis.Core.Parsers;

namespace ReportAnalysis.Core
{
    public class Ranger : IRanger
    {
        public DateRange GetRange(string path) => GetRanger(path).GetRange(path);

        private IRanger GetRanger(string path) => FormatDetector.GetFormat(path) switch
        {
            Format.Ararat => new AraratParser(),
            Format.Tinkoff => new TinkoffParser(),
            Format.Sber => new SberParser(),
            Format.Deniz => new DenizParser(),
            Format.Ziraat => new ZiraatParser(),
            Format.RawText => new ManualParser(),
            Format.ExpencesApp => new ExpensesAppParser(),
            _ => new StubParser()
        };
    }
}
