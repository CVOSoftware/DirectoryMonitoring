using System.Diagnostics;

namespace DirectoryMonitoring.Studio.Helper
{
    internal static class FileExplorerHelper
    {
        private const string APP_NAME = "explorer.exe";

        public static void OpenFileExplorer(string directoryPath)
        {
            var processStartInfo = new ProcessStartInfo();

            processStartInfo.FileName = APP_NAME;
            processStartInfo.Arguments = directoryPath;

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();
            }
        }
    }
}
