using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using CLI; // fixme
using PdfExtractor.Models;

namespace Viewer
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private bool _isNaOnly;
        public int _count;

        public event PropertyChangedEventHandler? PropertyChanged;

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

            UpdateOperationList();
        }

        public ICollection<TreeNode> Tree { get; } = new ObservableCollection<TreeNode>();

        public ICollection<Operation> List { get; } = new ObservableCollection<Operation>();

        public bool IsNaOnly
        {
            get { return _isNaOnly; }
            set
            {
                if (_isNaOnly != value)
                {
                    _isNaOnly = value;
                    UpdateOperationList();
                }
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        private void UpdateOperationList()
        {
            var text = File.ReadAllText("list.json") ;
            var list = JsonSerializer.Deserialize<Operation[]>(text);
            if (list != null)
            {
                List.Clear();
                foreach (var operation in list)
                {
                    if (IsNaOnly)
                    {
                        if (operation.Category == "n/a")
                        {
                            List.Add(operation);
                        }
                    }
                    else
                    {
                        List.Add(operation);
                    }
                }

                Count = List.Count;
            }
        }
    }
}
