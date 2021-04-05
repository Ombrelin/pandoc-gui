using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PandocGui.CliWrapper;
using PandocGui.Services;
using PandocGui.ViewModels;
using PandocGui.Views;

namespace PandocGui
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = new MainWindowViewModel(new FileDialogService(desktop.MainWindow), new PandocCli());
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}