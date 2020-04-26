using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectoryMonitoring.Studio.Base;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        #region Constructor

        public MainViewModel()
        {
            SettingVM = new SettingViewModel();
            SelectDirectoryVM = new SelectDirectoryViewModel();
            MonitorInfoVM = new MonitorInfoViewModel();
            OutputInfoVM = new OutputInfoViewModel();
        }

        #endregion

        #region Properties

        public SettingViewModel SettingVM { get; }

        public SelectDirectoryViewModel SelectDirectoryVM { get; }

        public MonitorInfoViewModel MonitorInfoVM { get; }

        public OutputInfoViewModel OutputInfoVM { get; }

        #endregion
    }
}
