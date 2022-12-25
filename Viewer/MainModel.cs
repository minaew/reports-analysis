using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfExtractor.Models;

namespace Viewer
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
            using var file = File.OpenRead(_filePath);
            var operations = JsonSerializer.Deserialize<List<Operation>>(file);
            return operations ?? Enumerable.Empty<Operation>();
        }

        public IEnumerable<TreeNode> GetOperationTree()
        {
            var years = new List<TreeNode>();

            foreach (var operation in GetOperationList())
            {
                var yearID = operation.DateTime.Year;
                var year = years.SingleOrDefault(y => y.ID == yearID);
                if (year == null)
                {
                    year = new TreeNode
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
                    month = new TreeNode
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
                    category = new TreeNode
                    {
                        ID = categoryID,
                        Title = operation.Category
                    };
                    month.SubCollection.Add(category);
                }

                category.SubCollection.Add(new TreeNode
                {
                    Title = $"{operation.DateTime} -- {operation.Account} -- {operation.Description}",
                    Money = (int)operation.Amount.Value
                });
                category.Money += (int)operation.Amount.Value;
            }

            return years;
        }
    }
}
