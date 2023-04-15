using PdfExtractor.Parsers;

namespace PdfExtractor
{
    public class MetaIdentifier : IIdentifier
    {
        public string? Identify(string path)
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
                default:
                    return "";
            }
        }
    }
}