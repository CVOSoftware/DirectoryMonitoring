using System.Collections.ObjectModel;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MonitorInfoViewModel : BaseViewModel
    {
        #region Field

        private bool autoScroll;

        #endregion

        #region Constructor

        public MonitorInfoViewModel()
        {
            Messenger.Default.Register<AddLogItemMessage>(this, AddLogItem);

            Logs = new ObservableCollection<LogViewModel>();
        }

        #endregion

        #region Properties

        public bool AutoScroll
        {
            get => autoScroll;
            set => SetValue(ref autoScroll, value);
        }

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
