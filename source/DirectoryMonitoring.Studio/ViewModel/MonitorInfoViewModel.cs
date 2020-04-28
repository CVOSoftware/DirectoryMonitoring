using System.Collections.ObjectModel;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using NLog.Fluent;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MonitorInfoViewModel : BaseViewModel
    {
        #region Field

        private bool scanComplete;

        private bool autoScroll;

        #endregion

        #region Constructor

        public MonitorInfoViewModel()
        {
            Messenger.Default.Register<AddLogItemMessage>(this, AddLogItem);
            Messenger.Default.Register<NotifyScanCompleteMessage>(this, ScanCanceled);
            Messenger.Default.Register<ClearLogsMessage>(this, ClearLogs);

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

        private void ScanCanceled(NotifyScanCompleteMessage message)
        {
            var informationToSave = Logs.Count > 0;

            scanComplete = message.ScanCanceled;
            Messenger.Default.Send<NotifyOutputInfoComponentMessage>(new NotifyOutputInfoComponentMessage(scanComplete, informationToSave));
        }

        private void ClearLogs(ClearLogsMessage message)
        {
            Logs.Clear();
        }

        #endregion

        #region Other methods

        private void SendNotifyOutputInfoMessage()
        {
            /*var 
            var message = new NotifyOutputInfoComponentMessage(scanCanceled, );
            Messenger.Default.Send<NotifyOutputInfoComponentMessage>(message);*/
        }

        #endregion
    }
}
