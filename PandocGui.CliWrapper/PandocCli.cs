using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PandocGui.CliWrapper.Command;
using Serilog;

namespace PandocGui.CliWrapper;

public class PandocCli : IPandocCli
{

    public async Task ExportPdfAsync(PandocParameters parameters)
    {
        var generator = BuildGenerator(parameters);

        int result = await ExecuteAsync(
            parameters.SourcePath,
            GetExecutionCommand(generator, parameters.SourcePath, parameters.TargetPath)
        );
        Log.Information($"Pandoc CLI return code : {result}");
        if (result != 0)
        {
            var error = (PandocErrorCode)result;
            Log.Error($"Pandoc Error : {error}");
            throw new InvalidOperationException($"{error}");
        }
    }

    public string GetCommand(PandocParameters parameters)
    {
        var generator = BuildGenerator(parameters);
        return GetExecutionCommand(generator, parameters.SourcePath, parameters.TargetPath);
    }


    private string GetExecutionCommand(IPandocCommandGenerator generator, string sourcePath, string targetPath)
    {
        var executionCommand = $"{generator.GetCommand(sourcePath)} -o \"{targetPath}\"";
        Log.Information($"Computed command : pandoc {executionCommand}");
        return executionCommand;
    }

    private async Task<int> ExecuteAsync(string sourcePath, string command)
    {
        string workingDirectory = new FileInfo(sourcePath).Directory.FullName;

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "pandoc",
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            }
        };

        process.OutputDataReceived += (s, e) => Log.Information($"Pandoc Info : {e.Data}");
        process.ErrorDataReceived += (s, e) => Log.Error($"Pandoc Error : {e.Data}");

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (process == null)
        {
            throw new ArgumentException("Invalid Command or pandoc not found");
        }

        await process.WaitForExitAsync();
        return process.ExitCode;
    }

    public IPandocCommandGenerator BuildGenerator(PandocParameters parameters)
    {
        Log.Information($"Building generator with parameters : {parameters}");
        var validationResult = new PandocParametersValidator().Validate(parameters);

        if (!validationResult.IsValid)
        {
            Log.Error("Invalid Parameters !");
            throw new ArgumentException("Invalid parameters");
        }

        IPandocCommandGenerator generator = new PandocCommandGenerator();

        if (parameters.HighlightTheme)
        {
            generator = new HighlightPandocCommandOptionsGenerator(generator, parameters.HighlightThemeSource);
        }

        if (parameters.NumberedHeader)
        {
            generator = new NumberedHeaderPandocCommandOptionsGenerator(generator);
        }

        if (parameters.CustomFont)
        {
            generator = new FontPandocCommandGenerator(generator, parameters.CustomFontName);
        }

        generator = new GeometryPandocCommandGenerator(generator, "a4paper");

        if (parameters.CustomMargin)
        {
            generator = new MarginPandocCommandGenerator(generator, parameters.CustomMarginValue);
        }

        if (parameters.CustomPdfEngine)
        {
            generator = new PdfEnginePandocCommandGenerator(generator, parameters.CustomPdfEngineValue);
        }

        if (parameters.TableOfContents)
        {
            generator = new ContentTablePandocCommandGenerator(generator);
        }

        if (parameters.LogToFile)
        {
            generator = new LogsFileCommandGenerator(generator, parameters.LogFilePath);
        }

        return generator;
    }
}