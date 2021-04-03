using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PandocGui.CliWrapper
{
    public abstract class PandocExecutableCommandGenerator : IPandocCommandGenerator
    {
        public abstract string GetCommand(string sourcePath);

        public string GetExecutionCommand(string sourcePath, string targetPath)
        {
            if (!targetPath.EndsWith(".pdf"))
            {
                throw new ArgumentException("Target should be a PDF");
            }

            return $"{GetCommand(sourcePath)} -o \"{targetPath}\"";
        }

        public string GetPandoc()
        {
            var pathEntries = ((Environment
                .GetEnvironmentVariable("PATH")
                ?.Split(';')) ?? throw new InvalidOperationException("PATH not found"));

            var pandocDir = pathEntries.First(entry =>
                Directory.Exists(entry) && Directory.GetFiles(entry).Any(file => file.Contains("pandoc"))
            );

            return Path.Combine(pandocDir,
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "pandoc.exe" : "pandoc");
        }

        public async Task ExecuteAsync(string sourcePath, string targetPath)
        {
            var pandoc = GetPandoc();
            var args = GetExecutionCommand(sourcePath, targetPath);
            var process = Process.Start(pandoc, args);
            if (process == null)
            {
                throw new ArgumentException("Invalid Command");
            }

            await process.WaitForExitAsync();

        }
    }
}