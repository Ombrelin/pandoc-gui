namespace PandocGui.CliWrapper
{
    public class PandocCommandGenerator : PandocExecutableCommandGenerator
    {

        public override string GetCommand(string sourcePath)
        {
            return $"-f markdown \"{sourcePath}\"";
        }
    }
}