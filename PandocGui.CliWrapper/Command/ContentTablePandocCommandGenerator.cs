namespace PandocGui.CliWrapper.Command;

public class ContentTablePandocCommandGenerator : PandocCommandWithOptionsGenerator
{
    public ContentTablePandocCommandGenerator(IPandocCommandGenerator commandGenerator) : base(commandGenerator)
    {
    }

    public override string GetCommand(string sourcePath) => $"{CommandGenerator.GetCommand(sourcePath)} --toc";
}