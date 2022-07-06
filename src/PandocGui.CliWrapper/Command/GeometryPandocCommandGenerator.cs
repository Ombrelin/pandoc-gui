namespace PandocGui.CliWrapper.Command;

public class GeometryPandocCommandGenerator : KeyValuePandocCommandOptionsGenerator
{
    public GeometryPandocCommandGenerator(IPandocCommandGenerator commandGenerator, string value) : base(
        commandGenerator, "geometry", value)
    {
    }
}