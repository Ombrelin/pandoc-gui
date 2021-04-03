namespace PandocGui.CliWrapper
{
    public class MarginPandocCommandGenerator : GeometryPandocCommandGenerator
    {
        public MarginPandocCommandGenerator(IPandocCommandGenerator commandGenerator, float value)
            : base(commandGenerator, $"margin={value.ToString(System.Globalization.CultureInfo.InvariantCulture)}cm")
        {
        }
    }
}