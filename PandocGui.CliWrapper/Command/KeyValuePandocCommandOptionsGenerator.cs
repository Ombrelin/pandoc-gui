namespace PandocGui.CliWrapper.Command;

public class KeyValuePandocCommandOptionsGenerator : PandocCommandWithOptionsGenerator
{
    private readonly string key;
    private readonly string value;

    public KeyValuePandocCommandOptionsGenerator(IPandocCommandGenerator commandGenerator, string key, string value)
        : base(commandGenerator)
    {
        this.key = key;
        this.value = value;
    }

    public override string GetCommand(string sourcePath) =>
        $"{CommandGenerator.GetCommand(sourcePath)} -V {key}:{value}";
}