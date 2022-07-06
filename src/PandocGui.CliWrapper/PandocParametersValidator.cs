using FluentValidation;

namespace PandocGui.CliWrapper;

public class PandocParametersValidator : AbstractValidator<PandocParameters>
{
    public PandocParametersValidator()
    {
        RuleFor(x => x.SourcePath).NotEmpty();
        RuleFor(x => x.TargetPath).NotEmpty();

        RuleFor(x => x.HighlightThemeSource).NotEmpty().When(x => x.HighlightTheme);
        RuleFor(x => x.CustomFontName).NotEmpty().When(x => x.CustomFont);
        RuleFor(x => x.CustomMarginValue).NotEmpty().When(x => x.CustomMargin);
        RuleFor(x => x.CustomPdfEngineValue).NotEmpty().When(x => x.CustomPdfEngine);
        RuleFor(x => x.LogFilePath).NotEmpty().When(x => x.LogToFile);
    }
}