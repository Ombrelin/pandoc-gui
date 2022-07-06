using System;
using System.Collections.Generic;

namespace PandocGui.CliWrapper.Command;

public class PdfEnginePandocCommandGenerator : PandocCommandWithOptionsGenerator
{
    private readonly string engine;

    public static HashSet<string> supportedEngines = new()
    {
        "pdflatex", "lualatex", "xelatex", "latexmk", "tectonic", "wkhtmltopdf", "weasyprint", "prince", "context",
        "pdfroff"
    };

    public PdfEnginePandocCommandGenerator(IPandocCommandGenerator commandGenerator, string engine) : base(
        commandGenerator)
    {
        this.engine = engine;

        if (!supportedEngines.Contains(this.engine))
        {
            throw new ArgumentException($"Unsupported PDF Engine : {this.engine}");
        }
    }

    public override string GetCommand(string sourcePath) =>
        $"{CommandGenerator.GetCommand(sourcePath)} --pdf-engine={this.engine}";
}