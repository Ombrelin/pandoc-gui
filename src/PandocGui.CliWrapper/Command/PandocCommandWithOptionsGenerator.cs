namespace PandocGui.CliWrapper.Command;

public abstract class PandocCommandWithOptionsGenerator : IPandocCommandGenerator
{
    public IPandocCommandGenerator CommandGenerator { get; }

    protected PandocCommandWithOptionsGenerator(IPandocCommandGenerator commandGenerator)
    {
        this.CommandGenerator = commandGenerator;
    }

    public abstract string GetCommand(string sourcePath);
}