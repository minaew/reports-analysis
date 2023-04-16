using System.Windows.Controls;

namespace ReportAnalysis.Viewer.Wpf
{
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            // var operation = (Operation) e.Row.Item;
            // if (operation.IsUnknownCategory)
            // {
            //     e.Row.Background = _unknownCategoryBrush;
            // }
            // else
            // {
            //     
            // }
        }
    }
}
