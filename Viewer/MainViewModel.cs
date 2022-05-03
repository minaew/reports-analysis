using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CLI; // fixme
using PdfExtractor.Models;

namespace Viewer
{
    internal class MainViewModel
    {
        public MainViewModel()
        {
            var text = File.ReadAllText("tree.json");
            var root = JsonSerializer.Deserialize<Root>(text, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
            if (root != null)
            {
                foreach (var year in root.Years.Select(year => new TreeNode
                {
                    Title = year.Key.ToString(),
                    SubCollection = year.Value.Months.Select(month => new TreeNode
                    {
                        Title = month.Key.ToString(),
                        SubCollection = month.Value.Categories.Select(category => new TreeNode
                        {
                            Title = category.Key,
                            SubCollection = category.Value.Operations.Select(operation => new TreeNode
                            {
                                Title = operation.Description
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }))
                {
                    Tree.Add(year);
                }
            }

            text = File.ReadAllText("list.json") ;
            var list = JsonSerializer.Deserialize<Operation[]>(text);
            if (list != null)
            {
                foreach (var operation in list)
                {
                    List.Add(operation);
                }
            }
        }

        public ICollection<TreeNode> Tree { get; } = new ObservableCollection<TreeNode>();

        public ICollection<Operation> List { get; } = new ObservableCollection<Operation>();
    }
}
