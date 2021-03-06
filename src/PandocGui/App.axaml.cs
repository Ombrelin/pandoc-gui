using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FluentAvalonia.Styling;
using PandocGui.CliWrapper;
using PandocGui.Services;
using PandocGui.ViewModels;
using PandocGui.Views;
using Serilog;

namespace PandocGui;

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
            var dataDirService = new DataDirectoryService();

            var theme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
            theme.RequestedTheme = "Dark";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                theme.ForceNativeTitleBarToTheme(desktop.MainWindow);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(@$"{dataDirService.GetLogsPath()}\app-logs-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.log")
                .CreateLogger();

            desktop.MainWindow.DataContext =
                new MainWindowViewModel(new FileDialogService(desktop.MainWindow),
                    new PandocCli(), dataDirService, this.Clipboard);
        }

        base.OnFrameworkInitializationCompleted();
    }
}