using System.Collections.Generic;
using PdfExtractor.Models;

namespace CLI
{
    internal class Root
    {
        public IDictionary<int, Year> Years { get; } = new Dictionary<int, Year>();
    }

    internal class Year
    {
        public IDictionary<int, Month> Months { get; } = new Dictionary<int, Month>();
    }

    internal class Month
    {
        public IDictionary<string, Category> Categories { get; } = new Dictionary<string, Category>();

    }

    internal class Category
    {
        public int Count => Operations.Count;


        // [JsonIgnore]
        public ICollection<Operation> Operations { get; } = new List<Operation>();
    }
}
