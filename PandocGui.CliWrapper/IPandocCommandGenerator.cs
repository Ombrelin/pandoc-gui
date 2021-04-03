using System.Threading.Tasks;

namespace PandocGui.CliWrapper
{
    public interface IPandocCommandGenerator
    {
        string GetCommand(string sourcePath);
        Task ExecuteAsync(string sourcePath, string targetPath);
    }
}