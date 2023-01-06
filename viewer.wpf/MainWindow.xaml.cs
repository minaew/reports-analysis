using System.Windows.Controls;
using System.Windows.Media;
using PdfExtractor.Models;

namespace Viewer.Wpf
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
