using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.ViewModel;
using DirectoryMonitoring.Studio.View;

namespace Studio
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var viewModel = new MainViewModel();
            var view = new MainWindow();

            view.DataContext = viewModel;
            view.Show();

        }
    }
}
