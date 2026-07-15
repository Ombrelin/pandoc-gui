using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using PandocGui.CliWrapper;
using PandocGui.CliWrapper.Command;
using PandocGui.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using SkiaSharp;

namespace PandocGui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> SearchSourceFileCommand { get; }
    public ReactiveCommand<Unit, Unit> ExportCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchTargetFileCommand { get; }
    public ReactiveCommand<Unit, Unit> SearchHighlightThemeSourceCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenLogFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }

    [Reactive] public partial string SourcePath { get; set; }

    [Reactive] public partial string TargetPath { get; set; }

    [Reactive] public partial bool CustomHighlightThemeEnabled { get; set; }

    [Reactive] public partial string CustomHighlightThemeSource { get; set; }

    [Reactive] public partial bool NumberedHeadersEnabled { get; set; }
    [Reactive] public partial bool CustomFontEnabled { get; set; }
    [Reactive] public partial string CustomFontName { get; set; }
    [Reactive] public partial bool CustomMarginEnabled { get; set; }
    [Reactive] public partial decimal CustomMarginValue { get; set; }
    [Reactive] public partial bool CustomPdfEngineEnabled { get; set; }
    [Reactive] public partial string CustomPdfEngineValue { get; set; }
    [Reactive] public partial bool TableOfContentEnabled { get; set; }
    [Reactive] public partial string Result { get; set; }
    [Reactive] public partial bool IsError { get; set; }
    [Reactive] public partial bool OpenFileOnCompletion { get; set; }
    public List<string> SupportedEngine { get; } = PdfEnginePandocCommandGenerator.supportedEngines.ToList();
    public List<string> InstalledFonts { get; }


    private readonly IFileDialogService fileDialogService;
    private readonly IPandocCli pandoc;
    private readonly IDataDirectoryService dataDirectoryService;
    private readonly IClipboard clipboard;
    private readonly ObservableAsPropertyHelper<bool> isExporting;
    public bool IsExporting => isExporting.Value;

    public MainWindowViewModel(IFileDialogService fileDialogService, IPandocCli pandoc,
        IDataDirectoryService dataDirectoryService, IClipboard clipboard)
    {
        this.clipboard = clipboard;
        dataDirectoryService.EnsureCreated();
        SourcePath = "";
        TargetPath = "";
        CustomHighlightThemeSource = "";
        CustomFontName = "";
        CustomMarginValue = 1.3m;
        CustomPdfEngineValue = "";
        Result = "";
        OpenFileOnCompletion = true;
        this.fileDialogService = fileDialogService;
        this.pandoc = pandoc;
        this.dataDirectoryService = dataDirectoryService;
        ClearCommand = ReactiveCommand.CreateFromTask(Clear);
        SearchSourceFileCommand = ReactiveCommand.CreateFromTask(SearchInputFile);
        SearchTargetFileCommand = ReactiveCommand.CreateFromTask(SearchOutputFile);
        ExportCommand = ReactiveCommand.CreateFromTask(Export);
        CopyCommand = ReactiveCommand.CreateFromTask(CopyPandocToClipBoard);
        ExportCommand.IsExecuting.ToProperty(this, x => x.IsExporting, out isExporting);
        SearchHighlightThemeSourceCommand = ReactiveCommand.CreateFromTask(SearchHighlightThemeSource);
        OpenLogFolderCommand = ReactiveCommand.Create(dataDirectoryService.OpenLogFolder);

        InstalledFonts = GetInstalledFonts();

        //auto set SourcePath as the file user opened.
        var args = Environment.GetCommandLineArgs();
        if (args.Count() > 1 && File.Exists(args[1]))
        {
            this.SourcePath= args[1];
        }
    }

    private static List<string> GetInstalledFonts() => SKFontManager.Default.FontFamilies.ToList();

    private async Task SearchHighlightThemeSource()
    {
        CustomHighlightThemeSource = await fileDialogService.OpenFileAsync();
    }

    private async Task Export()
    {
        try
        {
            this.Result = "";
            await this.pandoc.ExportPdfAsync(BuildPandocParameters());
            this.IsError = false;
            this.Result = "Success";

            Process.Start(new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "xdg-open" : TargetPath,
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? TargetPath : "",
                UseShellExecute = !RuntimeInformation.IsOSPlatform(OSPlatform.Linux),
                CreateNoWindow = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            });
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            this.IsError = true;
            this.Result = e.Message;
        }
    }

    private PandocParameters BuildPandocParameters()
    {
        return new PandocParameters()
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
                @$"{dataDirectoryService.GetLogsPath()}/pandoc-logs-{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.json"
        };
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

    private Task Clear()
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
        return Task.CompletedTask;
    }

    private Task CopyPandocToClipBoard()
    {
        clipboard.SetTextAsync($"pandoc {pandoc.GetCommand(BuildPandocParameters())}");
        return Task.CompletedTask;
    }
}