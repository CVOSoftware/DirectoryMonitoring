using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using DirectoryMonitoring.Studio.ViewModel;
using DirectoryMonitoring.Studio.View;
using MahApps.Metro.Controls.Dialogs;

namespace DirectoryMonitoring.Studio.Helper
{
    internal static class SaveDialogHelper
    {
        public static void SaveLog(string savePath, IEnumerable<LogViewModel> logs)
        {
            var view = new SaveWindow();

            view.ShowMessageAsync("test", "test");
        }
    }
}
