using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfExtractor.Models;

namespace Viewer.Wpf
{
    internal class MainModel
    {
        private readonly string _filePath;

        public MainModel(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<Operation> GetOperationList()
        {
            var content = File.ReadAllText(_filePath);
            var operations = JsonSerializer.Deserialize<List<Operation>>(content);
            return operations ?? Enumerable.Empty<Operation>();
        }

        public IEnumerable<ITreeNode> GetOperationTree()
        {
            var years = new List<ITreeNode>();

            foreach (var operation in GetOperationList())
            {
                var yearID = operation.DateTime.Year;
                var year = years.SingleOrDefault(y => y.ID == yearID);
                if (year == null)
                {
                    year = new InnerNode
                    {
                        ID = yearID,
                        Title = $"{yearID}"
                    };
                    years.Add(year);
                }

                var monthID = operation.DateTime.Month;
                var month = year.SubCollection.SingleOrDefault(m => m.ID == monthID);
                if (month == null)
                {
                    month = new InnerNode
                    {
                        ID = monthID,
                        Title = $"{monthID}"
                    };
                    year.SubCollection.Add(month);
                }

                var categoryID = operation.Category.GetHashCode();
                var category = month.SubCollection.SingleOrDefault(c => c.ID == categoryID);
                if (category == null)
                {
                    category = new InnerNode
                    {
                        ID = categoryID,
                        Title = operation.Category
                    };
                    month.SubCollection.Add(category);
                }

                category.SubCollection.Add(new EndNode
                {
                    Title = $"{operation.DateTime} -- {operation.Account} -- {operation.Description}",
                    Money = new AggregatedMoney(operation.Amount)
                });
                category.Money.Add(operation.Amount);
            }

            return years;
        }
    }
}
