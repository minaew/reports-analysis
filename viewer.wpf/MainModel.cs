using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Viewer.Wpf
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
                        Title = $"{yearID}",
                        Level = 0
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
                        Title = $"{monthID}",
                        Level = 1
                    };
                    year.SubCollection.Add(month);
                }

                if (string.IsNullOrEmpty(operation.Category))
                {
                    throw new InvalidOperationException();
                }
                var categoryID = operation.Category.GetHashCode();
                var category = month.SubCollection.SingleOrDefault(c => c.ID == categoryID);
                if (category == null)
                {
                    category = new InnerNode
                    {
                        ID = categoryID,
                        Title = operation.Category,
                        Level = 2
                    };
                    month.SubCollection.Add(category);
                }

                category.SubCollection.Add(new EndNode(
                    new AggregatedMoney(operation.Amount),
                    $"{operation.DateTime} -- {operation.Account} -- {operation.Description}"));
                category.Money.Add(operation.Amount);
            }

            return years;
        }
    }
}
