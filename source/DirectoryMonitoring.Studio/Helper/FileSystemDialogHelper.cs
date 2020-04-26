using System.Windows.Forms;

namespace DirectoryMonitoring.Studio.Helper
{
    internal static class FileSystemDialogHelper
    {
        public static string GetDirectory()
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                var result = folderBrowser.ShowDialog();

                return result == DialogResult.OK
                    ? folderBrowser.SelectedPath
                    : string.Empty;
            }
        }
    }
}
