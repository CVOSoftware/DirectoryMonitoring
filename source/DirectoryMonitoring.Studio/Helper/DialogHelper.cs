using System;
using System.Collections.Generic;
using System.Windows;
using MahApps.Metro.Controls;
using DirectoryMonitoring.Studio.ViewModel;
using DirectoryMonitoring.Studio.View;
using MahApps.Metro.Controls.Dialogs;

namespace DirectoryMonitoring.Studio.Helper
{
    internal static class DialogHelper
    {
        public static void MessageBox(string title, string description)
        {
            if(Application.Current.MainWindow is MetroWindow view)
            {
                view.ShowModalMessageExternal(title, description);
            }
        }
    }
}
