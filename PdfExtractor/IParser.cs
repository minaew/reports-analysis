using System.Collections.Generic;

namespace PdfExtractor
{
    public interface IParser
    {
        IReadOnlyList<Operation> Parse(string path);
    }
}
