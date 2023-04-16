using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Interfaces
{
    public interface ICategories
    {
        string GetCategory(Operation operation);
    }
}