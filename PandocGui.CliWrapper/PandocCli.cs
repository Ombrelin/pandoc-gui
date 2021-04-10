using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PandocGui.CliWrapper.Command;

namespace PandocGui.CliWrapper
{
    public class PandocCli : IPandocCli
    {
        private readonly ILogger logger;

        public PandocCli()
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("PandocGui.CliWrapper.PandocCli", LogLevel.Debug)
                    .AddConsole();
            });
            logger = loggerFactory.CreateLogger<PandocCli>();
        }

        public PandocCli(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task ExportPdfAsync(PandocParameters parameters)
        {
            var generator = BuildGenerator(parameters);

            int result =
                await ExecuteAsync(GetExecutionCommand(generator, parameters.SourcePath, parameters.TargetPath));
            logger.LogInformation($"Pandoc CLI return code : {result}");
            if (result != 0)
            {
                var error = (PandocErrorCode) result;
                logger.LogError($"Pandoc Eroor : {error}");
                throw new InvalidOperationException($"{error.ToString()}");
            }
        }


        public string GetExecutionCommand(IPandocCommandGenerator generator, string sourcePath, string targetPath)
        {
            if (!targetPath.EndsWith(".pdf"))
            {
                logger.LogError("Target should be a PDF");
                throw new ArgumentException("Target should be a PDF");
            }

            var executionCommand = $"{generator.GetCommand(sourcePath)} -o \"{targetPath}\"";
            logger.LogInformation($"Computer command : pandoc {executionCommand}");
            return executionCommand;
        }

        private async Task<int> ExecuteAsync(string command)
        {
            using Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = "pandoc",
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            ;
            process.OutputDataReceived += (s, e) => logger.LogInformation(e.Data);
            process.ErrorDataReceived += (s, e) => logger.LogError(e.Data);

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
            var validationResult = new PandocParametersValidator().Validate(parameters);

            if (!validationResult.IsValid)
            {
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
}