using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PandocGui.CliWrapper.Command
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



        public async Task<int> ExecuteAsync(string sourcePath, string targetPath)
        {
            using var process = Process.Start(
                new ProcessStartInfo()
                {
                    FileName = "pandoc",
                    Arguments = GetExecutionCommand(sourcePath, targetPath)
                });
            if (process == null)
            {
                throw new ArgumentException("Invalid Command or pandoc not found");
            }

            await process.WaitForExitAsync();
            return process.ExitCode;
        }
    }
}