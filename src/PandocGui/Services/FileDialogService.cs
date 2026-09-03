using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace PandocGui.Services;

public class FileDialogService : IFileDialogService
{
    private readonly Window window;

    public FileDialogService(Window window)
    {
        this.window = window;
    }

    public async Task<string> OpenFileAsync(IReadOnlyList<FilePickerGroup>? groups = null)
    {
        var options = new FilePickerOpenOptions { AllowMultiple = false };

        if (groups is { Count: > 0 })
        {
            var allExtensions = groups.SelectMany(group => group.Extensions).Distinct().ToList();

            var filters = new List<FilePickerFileType>
            {
                new("All supported documents") { Patterns = ToPatterns(allExtensions) }
            };
            filters.AddRange(groups.Select(group =>
                new FilePickerFileType(group.Name) { Patterns = ToPatterns(group.Extensions) }));
            filters.Add(FilePickerFileTypes.All);

            options.FileTypeFilter = filters;
        }

        try
        {
            var files = await window.StorageProvider.OpenFilePickerAsync(options);
            return files.Count == 0 ? "" : files[0].TryGetLocalPath() ?? "";
        }
        catch (Exception)
        {
            return "";
        }
    }

    public async Task<string> SaveFileAsync()
    {
        try
        {
            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions());
            return file?.TryGetLocalPath() ?? "";
        }
        catch (Exception)
        {
            return "";
        }
    }

    private static List<string> ToPatterns(IEnumerable<string> extensions) =>
        extensions.Select(extension => "*" + extension).ToList();
}
