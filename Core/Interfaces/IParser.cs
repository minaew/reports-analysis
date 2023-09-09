using System.Collections.Generic;
using System.IO;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Interfaces
{
    public interface IParser
    {
        IEnumerable<Operation> Parse(string path);

        IEnumerable<Operation> Parse(Stream stream);
    }

    public interface IIdentifier
    {
        string? Identify(string path);
    }

    public interface IRanger
    {
        DateRange GetRange(string path);

        DateRange GetRange(Stream stream);
    }
}
