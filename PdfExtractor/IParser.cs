using System.Collections.Generic;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public interface IParser
    {
        IEnumerable<Operation> Parse(string path);
    }

    public interface IIdentifier
    {
        string? Identify(string path);
    }
}
