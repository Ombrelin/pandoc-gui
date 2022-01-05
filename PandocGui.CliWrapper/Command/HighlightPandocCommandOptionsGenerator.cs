namespace PandocGui.CliWrapper.Command;

public class HighlightPandocCommandOptionsGenerator : PandocCommandWithOptionsGenerator
{
    private readonly string stylePath;

    public HighlightPandocCommandOptionsGenerator(IPandocCommandGenerator commandGenerator, string stylePath) :
        base(commandGenerator)
    {
        this.stylePath = stylePath;
    }

    public override string GetCommand(string sourcePath)
    {
        return $"{CommandGenerator.GetCommand(sourcePath)} --highlight-style \"{stylePath}\"";
    }
}