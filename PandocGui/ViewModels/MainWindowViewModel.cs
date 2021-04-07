using System;
using System.Collections.Generic;
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
        public ReactiveCommand<Unit, Task> SearchSourceFileCommand { get; }
        public ReactiveCommand<Unit, Task> ExportCommand { get; }
        public ReactiveCommand<Unit, Task> SearchTargetFileCommand { get; }
        public ReactiveCommand<Unit, Task> SearchHighlightThemeSourceCommand { get; }

        [Reactive] public string SourcePath { get; set; }

        [Reactive] public string TargetPath { get; set; }

        [Reactive] public bool Loading { get; set; } = false;

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

        public MainWindowViewModel(IFileDialogService fileDialogService, IPandocCli pandoc)
        {
            SourcePath = "";
            TargetPath = "";
            this.fileDialogService = fileDialogService;
            this.pandoc = pandoc;
            SearchSourceFileCommand = ReactiveCommand.Create(SearchInputFile);
            SearchTargetFileCommand = ReactiveCommand.Create(SearchOutputFile);
            ExportCommand = ReactiveCommand.Create(Export);
            SearchHighlightThemeSourceCommand = ReactiveCommand.Create(SearchHighlightThemeSource);
        }

        private async Task SearchHighlightThemeSource()
        {
            CustomHighlightThemeSource = await fileDialogService.OpenFileAsync();
        }

        private async Task Export()
        {
            Loading = true;
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
                    TableOfContents = TableOfContentEnabled
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
            finally
            {
                Loading = false;
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
    }
}