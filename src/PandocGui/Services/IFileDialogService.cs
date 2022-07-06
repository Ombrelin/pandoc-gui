using System.Threading.Tasks;

namespace PandocGui.Services;

public interface IFileDialogService
{
    Task<string> OpenFileAsync();
    Task<string> SaveFileAsync();
}