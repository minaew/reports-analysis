using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Viewer.Wpf
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly MainModel _model;
        private bool _isNaOnly = true;
        private int _count;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel(MainModel model)
        {
            _model = model;

            UpdateOperationList();
            UpdateOperationTree();
        }

        public ICollection<ITreeNode> Tree { get; } = new ObservableCollection<ITreeNode>();

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
            List.Clear();
            foreach (var operation in _model.GetOperationList())
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

        private void UpdateOperationTree()
        {
            Tree.Clear();
            foreach (var node in _model.GetOperationTree())
            {
                Tree.Add(node);
            }
        }
    }
}
