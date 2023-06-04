using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Viewer.Wpf
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly MainModel _model;
        private int _count;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(MainModel model)
        {
            _model = model;

            UpdateOperationList();
            UpdateOperationTree();
        }

        public ICollection<ITreeNode> Tree { get; } = new ObservableCollection<ITreeNode>();

        public ICollection<OperationViewModel> List { get; } = new ObservableCollection<OperationViewModel>();

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
            List.Clear();
            Categories.Clear();

            foreach (var operation in _model.GetOperationList())
            {
                List.Add(new OperationViewModel(operation));
                if (!Categories.Any(c => c.Name == operation.Category))
                {
                    Categories.Add(new CategoryViewModel(operation.Category) { Count = 1 });
                }
                else
                {
                    Categories.Single(c => c.Name == operation.Category).Count++;
                }

            }

            Count = List.Count;
        }

        public ObservableCollection<CategoryViewModel> Categories { get; } = new ObservableCollection<CategoryViewModel>();

        private void UpdateOperationTree()
        {
            Tree.Clear();
            foreach (var node in _model.GetOperationTree())
            {
                Tree.Add(node);
            }
        }
    }

    internal class OperationViewModel
    {
        public OperationViewModel(Operation operation)
        {
            MonthId = operation.DateTime.ToString("yyyy-MM");
            DateTime = operation.DateTime;
            Value = operation.Amount.Value;
            Currency = operation.Amount.Currency;
            Category = operation.Category;
            Description = operation.Description;
            Account = operation.Account;
        }

        public string MonthId { get; }

        public DateTime DateTime { get; }

        public double Value { get; }

        public string Currency { get; }

        public string Category { get; }

        public string Description { get; }

        public string Account { get; }
    }

    internal class CategoryViewModel
    {
        public CategoryViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int Count { get; set; }
    }
}
