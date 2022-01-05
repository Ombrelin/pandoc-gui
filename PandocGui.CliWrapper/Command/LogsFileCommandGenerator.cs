namespace PandocGui.CliWrapper.Command;

public class LogsFileCommandGenerator : PandocCommandWithOptionsGenerator
{
    private readonly string logFilePath;

    public LogsFileCommandGenerator(IPandocCommandGenerator commandGenerator, string logFilePath) : base(commandGenerator)
    {
        this.logFilePath = logFilePath;
    }

    public override string GetCommand(string sourcePath) => $"{CommandGenerator.GetCommand(sourcePath)} --log=\"{logFilePath}\"";
}