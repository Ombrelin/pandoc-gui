using System;
using System.Diagnostics;
using System.IO;

namespace PandocGui.Services;

public class DataDirectoryService : IDataDirectoryService
{
    private string DataDirectory => @$"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.pandoc-gui";
    private string DataDirectoryLogs => @$"{DataDirectory}\logs";


    public void EnsureCreated()
    {
        if (Directory.Exists(DataDirectory)) return;

        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(DataDirectoryLogs);
    }

    public void OpenLogFolder()
    {
        Console.WriteLine($"Opening : {DataDirectoryLogs}");
        Process.Start(new ProcessStartInfo()
        {
            FileName = DataDirectoryLogs,
            UseShellExecute = true,
            Verb = "open"
        });

    }

    public string GetPath()
    {
        return DataDirectory;
    }

    public string GetLogsPath()
    {
        return DataDirectoryLogs;
    }
}