using System.Threading.Tasks;

namespace PandocGui.CliWrapper.Command
{
    public interface IPandocCommandGenerator
    {
        string GetCommand(string sourcePath);
        Task<int> ExecuteAsync(string sourcePath, string targetPath);
    }
}