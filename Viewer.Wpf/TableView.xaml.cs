using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReportAnalysis.Viewer.Wpf
{
    public partial class TableView
    {
        private CollectionViewSource? _source;

        public TableView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (_source == null)
            {
                _source = (CollectionViewSource)sender;
            }
            var item = (OperationViewModel)e.Item;
            e.Accepted = true;

            if (CategoriesListBox.SelectedItems.Count > 0)
            {
                if (!CategoriesListBox.SelectedItems.Cast<CategoryViewModel>().Select(c => c.Name).Contains(item.Category))
                {
                    e.Accepted = false;
                }
            }
        }

        private void SelectedCategoriesChanged(object sender, SelectionChangedEventArgs args)
        {
            if (_source != null)
            {
                _source.View.Refresh();
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {

        }
    }
}
