using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectoryMonitoring.Studio.Base;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class LogViewModel : BaseViewModel
    {
        #region Constructor

        public LogViewModel(string fileEvent, string path, DateTime timestamp)
        {
            FileEvent = fileEvent;
            Path = path;
            Timestamp = timestamp;
        }

        #endregion

        #region Properties

        public string FileEvent { get; }

        public string Path { get; }

        public DateTime Timestamp { get; }

        #endregion
    }
}
