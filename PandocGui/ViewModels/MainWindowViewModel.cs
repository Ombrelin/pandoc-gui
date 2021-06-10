using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using PandocGui.CliWrapper;
using PandocGui.CliWrapper.Command;
using PandocGui.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Notification = System.Reactive.Notification;

namespace PandocGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> SearchSourceFileCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchTargetFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SearchHighlightThemeSourceCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenLogFolderCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearCommand { get; }

        [Reactive] public string SourcePath { get; set; }

        [Reactive] public string TargetPath { get; set; }

        [Reactive] public bool CustomHighlightThemeEnabled { get; set; } = false;

        [Reactive] public string CustomHighlightThemeSource { get; set; } = "";

        [Reactive] public bool NumberedHeadersEnabled { get; set; } = false;
        [Reactive] public bool CustomFontEnabled { get; set; } = false;
        [Reactive] public string CustomFontName { get; set; } = "";
        [Reactive] public bool CustomMarginEnabled { get; set; } = false;
        [Reactive] public decimal CustomMarginValue { get; set; } = 1.3m;
        [Reactive] public bool CustomPdfEngineEnabled { get; set; } = false;
        [Reactive] public string CustomPdfEngineValue { get; set; } = "";
        [Reactive] public bool TableOfContentEnabled { get; set; }
        [Reactive] public string Result { get; set; } = "";
        [Reactive] public bool IsError { get; set; } = false;
        public List<string> SupportedEngine { get; } = PdfEnginePandocCommandGenerator.supportedEngines.ToList();

        private readonly IFileDialogService fileDialogService;
        private readonly IPandocCli pandoc;
        private readonly IDataDirectoryService dataDirectoryService;

        private readonly ObservableAsPropertyHelper<bool> isExporting;
        public bool IsExporting => isExporting.Value;

        public MainWindowViewModel(IFileDialogService fileDialogService, IPandocCli pandoc,
            IDataDirectoryService dataDirectoryService)
        {
            dataDirectoryService.EnsureCreated();
            SourcePath = "";
            TargetPath = "";
            this.fileDialogService = fileDialogService;
            this.pandoc = pandoc;
            this.dataDirectoryService = dataDirectoryService;
            ClearCommand = ReactiveCommand.CreateFromTask(Clear);
            SearchSourceFileCommand = ReactiveCommand.CreateFromTask(SearchInputFile);
            SearchTargetFileCommand = ReactiveCommand.CreateFromTask(SearchOutputFile);
            ExportCommand = ReactiveCommand.CreateFromTask(Export);
            ExportCommand.IsExecuting.ToProperty(this, x => x.IsExporting, out isExporting);
            SearchHighlightThemeSourceCommand = ReactiveCommand.CreateFromTask(SearchHighlightThemeSource);
            OpenLogFolderCommand = ReactiveCommand.Create(dataDirectoryService.OpenLogFolder);
        }

        private async Task SearchHighlightThemeSource()
        {
            CustomHighlightThemeSource = await fileDialogService.OpenFileAsync();
        }

        private async Task Export()
        {
            try
            {
                this.Result = "";
                await this.pandoc.ExportPdfAsync(new PandocParameters()
                {
                    SourcePath = SourcePath,
                    TargetPath = TargetPath,
                    HighlightTheme = CustomHighlightThemeEnabled,
                    HighlightThemeSource = CustomHighlightThemeSource,
                    NumberedHeader = NumberedHeadersEnabled,
                    CustomFont = CustomFontEnabled,
                    CustomFontName = CustomFontName,
                    CustomMargin = CustomMarginEnabled,
                    CustomMarginValue = CustomMarginValue,
                    CustomPdfEngine = CustomPdfEngineEnabled,
                    CustomPdfEngineValue = CustomPdfEngineValue,
                    TableOfContents = TableOfContentEnabled,
                    LogToFile = true,
                    LogFilePath =
                        @$"{dataDirectoryService.GetLogsPath()}\pandoc-logs-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.json"
                });
                this.IsError = false;
                this.Result = "Success";
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
                this.IsError = true;
                this.Result = e.Message;
            }
        }

        private async Task SearchInputFile()
        {
            SourcePath = await fileDialogService.OpenFileAsync();
            Console.WriteLine($"Source path : {SourcePath}");
        }

        private async Task SearchOutputFile()
        {
            TargetPath = await fileDialogService.SaveFileAsync();
            Console.WriteLine($"Source path : {TargetPath}");
        }

        private async Task Clear()
        {
            SourcePath = "";
            TargetPath = "";

            CustomHighlightThemeEnabled = false;
            CustomHighlightThemeSource = "";
            NumberedHeadersEnabled = false;
            CustomFontEnabled = false;
            CustomFontName = "";
            CustomMarginEnabled = false;
            CustomMarginValue = 1.3m;
            CustomPdfEngineEnabled = false;
            CustomPdfEngineValue = "";
            TableOfContentEnabled = false;
        }
    }
}