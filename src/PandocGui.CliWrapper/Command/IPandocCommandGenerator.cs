namespace PandocGui.CliWrapper.Command;

public interface IPandocCommandGenerator
{
    string GetCommand(string sourcePath);

}