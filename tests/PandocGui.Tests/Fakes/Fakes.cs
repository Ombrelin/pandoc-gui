using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PandocGui.CliWrapper;
using PandocGui.Services;

namespace PandocGui.Tests.Fakes;

public class FakeFileDialogService : IFileDialogService
{
    public string OpenFileResult { get; set; } = "";
    public string SaveFileResult { get; set; } = "";
    public IReadOnlyList<FilePickerGroup> LastOpenFileGroups { get; private set; }

    public Task<string> OpenFileAsync(IReadOnlyList<FilePickerGroup> groups = null)
    {
        LastOpenFileGroups = groups;
        return Task.FromResult(OpenFileResult);
    }

    public Task<string> SaveFileAsync() => Task.FromResult(SaveFileResult);
}

public class FakePandocCli : IPandocCli
{
    public PandocParameters LastParameters { get; private set; }
    public Exception ExportException { get; set; }
    public string Command { get; set; } = "-f markdown \"source.md\"";

    public Task ExportPdfAsync(PandocParameters parameters)
    {
        LastParameters = parameters;
        return ExportException is null ? Task.CompletedTask : Task.FromException(ExportException);
    }

    public string GetCommand(PandocParameters parameters)
    {
        LastParameters = parameters;
        return Command;
    }
}

public class FakeDataDirectoryService : IDataDirectoryService
{
    public int EnsureCreatedCalls { get; private set; }
    public int OpenLogFolderCalls { get; private set; }

    public void EnsureCreated() => EnsureCreatedCalls++;
    public void OpenLogFolder() => OpenLogFolderCalls++;
    public string GetPath() => "data";
    public string GetLogsPath() => "data/logs";
}

public class FakeClipboardService : IClipboardService
{
    public string LastText { get; private set; }

    public Task SetTextAsync(string text)
    {
        LastText = text;
        return Task.CompletedTask;
    }
}
