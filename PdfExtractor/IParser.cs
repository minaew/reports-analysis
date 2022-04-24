using System.Collections.Generic;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public interface IParser
    {
        IReadOnlyList<Operation> Parse(string path);
    }
}
