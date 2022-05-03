using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Viewer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var path = desktop.Args[0];
                var model = new MainModel(path);
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(model)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}