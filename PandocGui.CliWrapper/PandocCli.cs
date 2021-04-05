using System;
using System.Threading.Tasks;
using PandocGui.CliWrapper.Command;

namespace PandocGui.CliWrapper
{
    public class PandocCli : IPandocCli
    {
        public async Task ExportPdfAsync(PandocParameters parameters)
        {
            var generator = BuildGenerator(parameters);

            int result = await generator.ExecuteAsync(parameters.SourcePath, parameters.TargetPath);
            if (result != 0)
            {
                throw new InvalidOperationException($"Pandoc error : {result}");
            }
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

            return generator;
        }
    }
}