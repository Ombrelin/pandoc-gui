using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
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
                var dataDirService = new DataDirectoryService();
                desktop.MainWindow.DataContext =
                    new MainWindowViewModel(new FileDialogService(desktop.MainWindow), new PandocCli(GetLogger(dataDirService)),dataDirService );
            }

            base.OnFrameworkInitializationCompleted();
        }

        private ILogger GetLogger(IDataDirectoryService dataDirectoryService)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("PandocGui.PandocGui.App", LogLevel.Debug)
                        .AddFile(
                            @$"{dataDirectoryService.GetLogsPath()}\app-logs-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.log")
                        .AddConsole();
                });
                return loggerFactory.CreateLogger<App>();
            }
    }
}