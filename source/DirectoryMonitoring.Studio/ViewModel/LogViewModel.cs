using System;
using DirectoryMonitoring.Studio.Base;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class LogViewModel : BaseViewModel
    {
        #region Constructor

        public LogViewModel(string fileEvent, string path, DateTime timestamp)
        {
            FileEvent = fileEvent;
            Timestamp = timestamp;
            Path = path;
        }

        #endregion

        #region Properties

        public string FileEvent { get; }

        public DateTime Timestamp { get; }

        public string Path { get; }

        #endregion
    }
}
