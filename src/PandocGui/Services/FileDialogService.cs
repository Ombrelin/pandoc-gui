using System;
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

    public async Task<string> OpenFileAsync()
    {
        try
        {
            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false
            });

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
}
