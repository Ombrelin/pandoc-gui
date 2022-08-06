using System;
using System.IO;
using System.Threading.Tasks;
using PandocGui.CliWrapper;
using PandocGui.CliWrapper.Command;
using Xunit;

namespace PandocGui.Tests;

public class PandocCommandGeneratorTest
{
    [Fact]
    public void CommandGenerator_ReturnsBaseCommand()
    {
        // Given
        var generator = new PandocCommandGenerator();

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\"", command);
    }

    [Fact]
    public void HighlightGenerator_ReturnsHighlightCommand()
    {
        // Given
        var generator = new HighlightPandocCommandOptionsGenerator(new PandocCommandGenerator(), "style.theme");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" --highlight-style \"style.theme\"", command);
    }


    [Fact]
    public void NumberedHeaderGenerator_ReturnsNumberedHeaderCommand()
    {
        // Given
        var generator = new NumberedHeaderPandocCommandOptionsGenerator(new PandocCommandGenerator());

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" -N", command);
    }

    [Fact]
    public void KeyValueGenerator_ReturnsKeyValueCommand()
    {
        // Given
        var generator =
            new KeyValuePandocCommandOptionsGenerator(new PandocCommandGenerator(), "testkey", "testvalue");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" -V testkey:testvalue", command);
    }

    [Fact]
    public void FontGenerator_ReturnsFontCommand()
    {
        // Given
        var generator = new FontPandocCommandGenerator(new PandocCommandGenerator(), "Segoe UI");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" -V mainfont:\"Segoe UI\"", command);
    }

    [Fact]
    public void GeometryGenerator_ReturnsGeometryCommand()
    {
        // Given
        var generator = new GeometryPandocCommandGenerator(new PandocCommandGenerator(), "a4paper");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" -V geometry:a4paper", command);
    }

    [Fact]
    public void MarginGenerator_ReturnsMarginCommand()
    {
        // Given
        var generator = new MarginPandocCommandGenerator(new PandocCommandGenerator(), 1.3m);

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" -V geometry:margin=1.3cm", command);
    }

    [Fact]
    public void TocGenerator_ReturnsTocCommand()
    {
        // Given
        var generator = new ContentTablePandocCommandGenerator(new PandocCommandGenerator());

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" --toc", command);
    }

    [Fact]
    public void EngineGenerator_ExistingEngine_ReturnsEngineCommand()
    {
        // Given
        var generator = new PdfEnginePandocCommandGenerator(new PandocCommandGenerator(), "xelatex");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" --pdf-engine=xelatex", command);
    }

    [Fact]
    public void LogGenerator_ReturnsLogCommand()
    {
        // Given
        var generator = new LogsFileCommandGenerator(new PandocCommandGenerator(), "logs.txt");

        // When
        var command = generator.GetCommand("test.md");

        // Then
        Assert.Equal("-f markdown \"test.md\" --log=\"logs.txt\"", command);
    }

    [Fact]
    public void EngineGenerator_NonExistingEngine_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new PdfEnginePandocCommandGenerator(new PandocCommandGenerator(), "not a valid engine"));
    }

    [Fact]
    public void BuildCommandFromParameters_ProducesCorrectCommand()
    {
        // Given
        var parameters = new PandocParameters()
        {
            SourcePath = "test.md",
            TargetPath = "test.pdf",
            HighlightTheme = true,
            HighlightThemeSource = "style.theme",
            NumberedHeader = true,
            CustomFont = true,
            CustomFontName = "Segoe UI",
            CustomMargin = true,
            CustomMarginValue = 1.3m,
            CustomPdfEngine = true,
            CustomPdfEngineValue = "xelatex",
            TableOfContents = true,
            LogToFile = true,
            LogFilePath = "logs.txt"
        };

        var cli = new PandocCli();

        // When
        var generator = cli.BuildGenerator(parameters);

        // Then
        Assert.Equal(
            "-f markdown \"test.md\" --highlight-style \"style.theme\" -N -V mainfont:\"Segoe UI\" -V geometry:a4paper -V geometry:margin=1.3cm --pdf-engine=xelatex --toc --log=\"logs.txt\"",
            generator.GetCommand("test.md"));
    }
}