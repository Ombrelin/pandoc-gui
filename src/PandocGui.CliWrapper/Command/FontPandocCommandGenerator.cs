namespace PandocGui.CliWrapper.Command;

public class FontPandocCommandGenerator : KeyValuePandocCommandOptionsGenerator
{
    public FontPandocCommandGenerator(IPandocCommandGenerator commandGenerator, string value)
        : base(commandGenerator, "mainfont", $"\"{value}\"")
    {
    }
}