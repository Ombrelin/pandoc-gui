namespace PandocGui.Services;

public interface IDataDirectoryService
{
    void EnsureCreated();
    void OpenLogFolder();
    string GetPath();
    string GetLogsPath();
}