using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Parsers;

namespace ReportAnalysis.Core
{
    public class Identifier : IIdentifier
    {
        public string Identify(string path)
        {
            switch (FormatDetector.GetFormat(path))
            {
                case Models.Format.Ararat:
                    return new AraratParser().Identify(path);
                case Models.Format.Deniz:
                    return new DenizParser().Identify(path);
                case Models.Format.Ziraat:
                    return new ZiraatParser().Identify(path);
                case Models.Format.ExpencesApp:
                    return new ExpensesAppParser().Identify(path);
                case Models.Format.Tinkoff:
                    return new TinkoffParser().Identify(path);
                case Models.Format.RawText:
                    return "manual";
                case Models.Format.Sber:
                    return new SberParser().Identify(path);
                default:
                    return "";
            }
        }
    }
}