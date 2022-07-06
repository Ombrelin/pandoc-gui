namespace PandocGui.CliWrapper.Command;

public class PandocCommandGenerator : IPandocCommandGenerator
{
    public string GetCommand(string sourcePath)
    {
        return $"-f markdown \"{sourcePath}\"";
    }
}