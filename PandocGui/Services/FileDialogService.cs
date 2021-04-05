using System.Threading.Tasks;
using Avalonia.Controls;

namespace PandocGui.Services
{
    public class FileDialogService : IFileDialogService
    {
        private readonly Window window;

        public FileDialogService(Window window)
        {
            this.window = window;
        }

        public async Task<string> OpenFileAsync()
        {
            var dialog = new OpenFileDialog()
            {
                AllowMultiple = false
            };
            var files = await dialog.ShowAsync(this.window);
            return files[0];
        }

        public async Task<string> SaveFileAsync()
        {
            var dialog = new SaveFileDialog();
            var file = await dialog.ShowAsync(this.window);
            return file;
        }
    }
}