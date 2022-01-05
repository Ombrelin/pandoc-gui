using System.Globalization;

namespace PandocGui.CliWrapper.Command;

public class MarginPandocCommandGenerator : GeometryPandocCommandGenerator
{
    public MarginPandocCommandGenerator(IPandocCommandGenerator commandGenerator, decimal value)
        : base(commandGenerator, $"margin={value.ToString(CultureInfo.InvariantCulture)}cm")
    {
    }
}