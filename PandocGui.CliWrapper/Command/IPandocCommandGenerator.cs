using System.Threading.Tasks;

namespace PandocGui.CliWrapper.Command;

public interface IPandocCommandGenerator
{
    string GetCommand(string sourcePath);

}