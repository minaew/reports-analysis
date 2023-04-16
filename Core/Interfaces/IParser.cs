using System.Collections.Generic;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Interfaces
{
    public interface IParser
    {
        IEnumerable<Operation> Parse(string path);
    }

    public interface IIdentifier
    {
        string? Identify(string path);
    }

    public interface IRanger
    {
        DateRange GetRange(string path);
    }
}
