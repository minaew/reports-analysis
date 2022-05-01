using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using CLI; // fixme

namespace Viewer
{
    internal class TreeNode
    {
        public string Title { get; set; } = string.Empty;

        public ICollection<TreeNode> SubCollection { get; set; } = new List<TreeNode>();
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // FIXME: vreate viewmodel
            var text = File.ReadAllText("tree.json");
            var root = JsonSerializer.Deserialize<Root>(text, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            var collection = root.Years.Select(year => new TreeNode
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
            });

            var tree = this.FindControl<TreeView>("Tree");
            tree.Items = collection;
        }
    }
}