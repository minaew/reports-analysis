using System.Windows;

namespace ReportAnalysis.Viewer.Wpf
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length == 0)
            {
                new MainWindow().Show();
            }
            else
            {
                var path = e.Args[0];
                var model = new MainModel(path);
                new MainWindow
                {
                    DataContext = new MainViewModel(model)
                }.Show();
            }
        }
    }
}
