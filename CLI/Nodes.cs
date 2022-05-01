using System.Collections.Generic;
using PdfExtractor.Models;

namespace CLI
{
    public class Root
    {
        public IDictionary<int, Year> Years { get; set; } = new Dictionary<int, Year>();
    }

    public class Year
    {
        public IDictionary<int, Month> Months { get; set; } = new Dictionary<int, Month>();
    }

    public class Month
    {
        public IDictionary<string, Category> Categories { get; set; } = new Dictionary<string, Category>();

    }

    public class Category
    {
        public int Count => Operations.Count;


        // [JsonIgnore]
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    }
}
