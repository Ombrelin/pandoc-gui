using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PandocGui.CliWrapper;
using PandocGui.Tests.Fakes;
using PandocGui.ViewModels;
using Xunit;

namespace PandocGui.Tests;

public class MainWindowViewModelTest
{
    private readonly FakeFileDialogService fileDialog = new();
    private readonly FakePandocCli pandoc = new();
    private readonly FakeDataDirectoryService dataDirectory = new();
    private readonly FakeClipboardService clipboard = new();

    private MainWindowViewModel BuildViewModel() =>
        new(fileDialog, pandoc, dataDirectory, clipboard);

    [Fact]
    public void NewViewModel_StartsEmptyWithDefaultFormats()
    {
        // When
        var viewModel = BuildViewModel();

        // Then
        Assert.Equal("", viewModel.SourcePath);
        Assert.Equal("", viewModel.TargetPath);
        Assert.Equal(PandocFormats.DefaultInputFormat, viewModel.SelectedInputFormat.Format);
        Assert.Equal(PandocFormats.OutputFormats[0], viewModel.SelectedOutputFormat);
        Assert.Equal(1, dataDirectory.EnsureCreatedCalls);
    }

    [Theory]
    [InlineData("report.md", "markdown")]
    [InlineData("paper.docx", "docx")]
    [InlineData("page.HTML", "html")]
    [InlineData("thesis.tex", "latex")]
    [InlineData("archive.unknown", "markdown")]
    public void SettingSourcePath_DetectsInputFormat(string fileName, string expectedFormat)
    {
        // Given
        var viewModel = BuildViewModel();

        // When
        viewModel.SourcePath = Path.Combine("documents", fileName);

        // Then
        Assert.Equal(expectedFormat, viewModel.SelectedInputFormat.Format);
    }

    [Fact]
    public void SettingSourcePath_FillsTargetPathWithSelectedOutputExtension()
    {
        // Given
        var viewModel = BuildViewModel();

        // When
        viewModel.SourcePath = Path.Combine("documents", "report.md");

        // Then
        Assert.Equal(Path.Combine("documents", "report.pdf"), viewModel.TargetPath);
    }

    [Fact]
    public void SettingSourcePath_ToBlank_LeavesTargetPathUntouched()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "report.md");

        // When
        viewModel.SourcePath = "";

        // Then
        Assert.Equal(Path.Combine("documents", "report.pdf"), viewModel.TargetPath);
        Assert.Equal("markdown", viewModel.SelectedInputFormat.Format);
    }

    [Fact]
    public void ChangingOutputFormat_SwapsTargetExtensionKeepingFolderAndName()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "report.md");

        // When
        viewModel.SelectedOutputFormat = PandocFormats.OutputFormats.First(format => format.Extension == ".html");

        // Then
        Assert.Equal(Path.Combine("documents", "report.html"), viewModel.TargetPath);
    }

    [Fact]
    public void ChangingOutputFormat_WithoutTargetPath_FillsItFromSourcePath()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "report.md");
        viewModel.TargetPath = "";

        // When
        viewModel.SelectedOutputFormat = PandocFormats.OutputFormats.First(format => format.Extension == ".epub");

        // Then
        Assert.Equal(Path.Combine("documents", "report.epub"), viewModel.TargetPath);
    }

    [Theory]
    [InlineData("", "", false)]
    [InlineData("source.md", "", false)]
    [InlineData("", "target.pdf", false)]
    [InlineData("source.md", "target.pdf", true)]
    public async Task ExportCommand_IsEnabledOnlyWithBothPaths(string source, string target, bool expected)
    {
        // Given
        var viewModel = BuildViewModel();

        // When
        viewModel.SourcePath = source;
        viewModel.TargetPath = target;

        // Then
        Assert.Equal(expected, await viewModel.ExportCommand.CanExecute.FirstAsync());
    }

    [Fact]
    public async Task ExportCommand_WhenPandocFails_ReportsTheError()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "report.md");
        pandoc.ExportException = new InvalidOperationException("pandoc exploded");

        // When
        await viewModel.ExportCommand.Execute();

        // Then
        Assert.True(viewModel.IsError);
        Assert.Equal("pandoc exploded", viewModel.Result);
    }

    [Fact]
    public async Task CopyCommand_PutsTheFullPandocCommandOnTheClipboard()
    {
        // Given
        var viewModel = BuildViewModel();
        pandoc.Command = "-f docx \"paper.docx\"";

        // When
        await viewModel.CopyCommand.Execute();

        // Then
        Assert.Equal("pandoc -f docx \"paper.docx\"", clipboard.LastText);
    }

    [Fact]
    public async Task CopyCommand_BuildsParametersFromTheCurrentSelection()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "paper.docx");
        viewModel.NumberedHeadersEnabled = true;
        viewModel.TableOfContentEnabled = true;

        // When
        await viewModel.CopyCommand.Execute();

        // Then
        var parameters = pandoc.LastParameters;
        Assert.Equal(Path.Combine("documents", "paper.docx"), parameters.SourcePath);
        Assert.Equal(Path.Combine("documents", "paper.pdf"), parameters.TargetPath);
        Assert.Equal("docx", parameters.SourceFormat);
        Assert.True(parameters.NumberedHeader);
        Assert.True(parameters.TableOfContents);
        Assert.StartsWith(dataDirectory.GetLogsPath(), parameters.LogFilePath);
    }

    [Fact]
    public async Task SearchSourceFileCommand_TakesThePathFromTheFileDialog()
    {
        // Given
        var viewModel = BuildViewModel();
        fileDialog.OpenFileResult = Path.Combine("documents", "notes.rst");

        // When
        await viewModel.SearchSourceFileCommand.Execute();

        // Then
        Assert.Equal(Path.Combine("documents", "notes.rst"), viewModel.SourcePath);
        Assert.Equal("rst", viewModel.SelectedInputFormat.Format);
    }

    [Fact]
    public async Task SearchSourceFileCommand_GroupsFormatsByCategoryForThePicker()
    {
        // Given
        var viewModel = BuildViewModel();

        // When
        await viewModel.SearchSourceFileCommand.Execute();

        // Then
        var groups = fileDialog.LastOpenFileGroups;
        Assert.NotNull(groups);
        Assert.NotEmpty(groups);
        Assert.Contains(groups, group => group.Name == "Markdown" && group.Extensions.Contains(".md"));
        Assert.All(groups, group => Assert.NotEmpty(group.Extensions));
    }

    [Fact]
    public async Task SearchTargetFileCommand_TakesThePathFromTheFileDialog()
    {
        // Given
        var viewModel = BuildViewModel();
        fileDialog.SaveFileResult = Path.Combine("documents", "notes.epub");

        // When
        await viewModel.SearchTargetFileCommand.Execute();

        // Then
        Assert.Equal(Path.Combine("documents", "notes.epub"), viewModel.TargetPath);
    }

    [Fact]
    public async Task ClearCommand_ResetsEveryOption()
    {
        // Given
        var viewModel = BuildViewModel();
        viewModel.SourcePath = Path.Combine("documents", "paper.docx");
        viewModel.SelectedOutputFormat = PandocFormats.OutputFormats.First(format => format.Extension == ".html");
        viewModel.CustomHighlightThemeEnabled = true;
        viewModel.CustomHighlightThemeSource = "theme.theme";
        viewModel.NumberedHeadersEnabled = true;
        viewModel.CustomFontEnabled = true;
        viewModel.CustomFontName = "Arial";
        viewModel.CustomMarginEnabled = true;
        viewModel.CustomMarginValue = 2.5m;
        viewModel.CustomPdfEngineEnabled = true;
        viewModel.CustomPdfEngineValue = "xelatex";
        viewModel.TableOfContentEnabled = true;
        viewModel.Result = "Success";
        viewModel.IsError = true;

        // When
        await viewModel.ClearCommand.Execute();

        // Then
        Assert.Equal("", viewModel.SourcePath);
        Assert.Equal("", viewModel.TargetPath);
        Assert.Equal(PandocFormats.DefaultInputFormat, viewModel.SelectedInputFormat.Format);
        Assert.Equal(PandocFormats.OutputFormats[0], viewModel.SelectedOutputFormat);
        Assert.Equal("", viewModel.Result);
        Assert.False(viewModel.IsError);
        Assert.False(viewModel.CustomHighlightThemeEnabled);
        Assert.Equal("", viewModel.CustomHighlightThemeSource);
        Assert.False(viewModel.NumberedHeadersEnabled);
        Assert.False(viewModel.CustomFontEnabled);
        Assert.Equal("", viewModel.CustomFontName);
        Assert.False(viewModel.CustomMarginEnabled);
        Assert.Equal(1.3m, viewModel.CustomMarginValue);
        Assert.False(viewModel.CustomPdfEngineEnabled);
        Assert.Equal("", viewModel.CustomPdfEngineValue);
        Assert.False(viewModel.TableOfContentEnabled);
    }

    [Fact]
    public void HasInput_FollowsWhetherASourcePathIsSet()
    {
        // Given
        var viewModel = BuildViewModel();
        Assert.False(viewModel.HasInput);

        // When
        viewModel.SourcePath = Path.Combine("documents", "report.md");

        // Then
        Assert.True(viewModel.HasInput);

        // And when
        viewModel.SourcePath = "   ";

        // Then
        Assert.False(viewModel.HasInput);
    }

    [Fact]
    public void IsStatusVisible_FollowsWhetherAResultIsSet()
    {
        // Given
        var viewModel = BuildViewModel();
        Assert.False(viewModel.IsStatusVisible);

        // When
        viewModel.Result = "Success";

        // Then
        Assert.True(viewModel.IsStatusVisible);

        // And when
        viewModel.Result = "";

        // Then
        Assert.False(viewModel.IsStatusVisible);
    }

    [Fact]
    public void OpenLogFolderCommand_AsksTheDataDirectoryService()
    {
        // Given
        var viewModel = BuildViewModel();

        // When
        viewModel.OpenLogFolderCommand.Execute().Subscribe();

        // Then
        Assert.Equal(1, dataDirectory.OpenLogFolderCalls);
    }
}
