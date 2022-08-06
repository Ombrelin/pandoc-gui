using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

            FluentAvaloniaTheme theme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>() ?? throw new InvalidOperationException("Can't get Fluent Avalonia Theme");
            theme.RequestedTheme = "Dark";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                theme.ForceNativeTitleBarToTheme(desktop.MainWindow);
            }

            theme.CustomAccentColor = Color.FromRgb(0, 102, 204);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(@$"{dataDirService.GetLogsPath()}/app-logs-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.log")
                .CreateLogger();

            desktop.MainWindow.DataContext = new MainWindowViewModel(
                    new FileDialogService(desktop.MainWindow),
                    new PandocCli(),
                    dataDirService,
                    this.Clipboard ?? throw new InvalidOperationException("No application clipboard")
                );
        }

        base.OnFrameworkInitializationCompleted();
    }
}