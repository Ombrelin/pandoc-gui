using System.Threading.Tasks;

namespace PandocGui.CliWrapper;

public interface IPandocCli
{
    Task ExportPdfAsync(PandocParameters parameters);
    string GetCommand(PandocParameters parameters);
}