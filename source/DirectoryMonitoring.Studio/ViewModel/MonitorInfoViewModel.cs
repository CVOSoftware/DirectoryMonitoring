using System.Collections.ObjectModel;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MonitorInfoViewModel : BaseViewModel
    {
        #region Constructor

        public MonitorInfoViewModel()
        {
            Messenger.Default.Register<AddLogItemMessage>(this, AddLogItem);

            Logs = new ObservableCollection<LogViewModel>();
        }

        #endregion

        #region Properties

        public ObservableCollection<LogViewModel> Logs { get; private set; }

        #endregion

        #region Message handlers

        private void AddLogItem(AddLogItemMessage message)
        {
            var log = new LogViewModel(message.FileEvent, message.Path, message.Timestamp);

            Logs.Add(log);
        }

        #endregion
    }
}
