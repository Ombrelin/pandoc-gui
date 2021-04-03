namespace PandocGui.CliWrapper
{
    public class FontPandocCommandGenerator : KeyValuePandocCommandOptionsGenerator
    {
        public FontPandocCommandGenerator(IPandocCommandGenerator commandGenerator, string value) 
            : base(commandGenerator, "mainfont", value)
        {
        }
    }
}