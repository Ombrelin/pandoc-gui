namespace PandocGui.CliWrapper
{
    public class KeyValuePandocCommandOptionsGenerator : PandocCommandWithOptionsGenerator
    {
        private string key;
        private string value;
        
        public KeyValuePandocCommandOptionsGenerator(IPandocCommandGenerator commandGenerator, string key, string value) : base(commandGenerator)
        {
            this.key = key;
            this.value = value;
        }

        public override string GetCommand(string sourcePath) => $"{CommandGenerator.GetCommand(sourcePath)} -V {key}:{value}";
    }
}