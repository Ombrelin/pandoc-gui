using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using PandocGui.CliWrapper;
using PandocGui.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PandocGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public ReactiveCommand<Unit, Task> SearchSourceFileCommand { get; }
        public ReactiveCommand<Unit, Task> ExportCommand { get; }
        public ReactiveCommand<Unit,Task> SearchTargetFileCommand { get; }
        public ReactiveCommand<Unit,Task> SearchHighlightThemeSourceCommand { get; }

        [Reactive]
        public string SourcePath { get; set; }
        
        [Reactive]
        public string TargetPath { get; set; }

        [Reactive] 
        public bool Loading { get; set; } = false;

        [Reactive] 
        public bool CustomHighlightThemeEnabled { get; set; } = false;

        [Reactive] 
        public string CustomHighlightThemeSource { get; set; } = "";
        
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
            await this.pandoc.ExportPdfAsync(new PandocParameters()
            {
                SourcePath = SourcePath,
                TargetPath = TargetPath,
                HighlightTheme = CustomHighlightThemeEnabled,
                HighlightThemeSource = CustomHighlightThemeSource,
                NumberedHeader = false,
                CustomFont = false,
                CustomMargin = false,
                CustomPdfEngine = false,
                TableOfContents = false
            });
            Loading = false;
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