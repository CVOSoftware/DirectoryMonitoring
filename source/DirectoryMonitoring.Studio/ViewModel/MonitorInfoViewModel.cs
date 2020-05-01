using System;
using System.Windows;
using System.Collections.ObjectModel;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MonitorInfoViewModel : BaseViewModel
    {
        #region Const 

        private const string FILE_EXTENSION = ".txt";

        private const string TITLE = "Saving complete";

        private const string LABEL = "Saved to path:";

        #endregion

        #region Field

        private bool scanComplete;

        private bool autoScroll;

        private bool saving;

        #endregion

        #region Constructor

        public MonitorInfoViewModel()
        {
            Messenger.Default.Register<AddLogItemMessage>(this, AddLogItem);
            Messenger.Default.Register<NotifyScanCompleteMessage>(this, ScanCanceled);
            Messenger.Default.Register<ClearLogsMessage>(this, ClearLogsHandler);
            Messenger.Default.Register<SaveLogMessage>(this, SaveLogHandler);

            saving = true;
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

        #region Command

        #region Save

        private RelayCommand clear;

        public RelayCommand Clear => RelayCommand.Register(ref clear, OnClear, CanClear);

        private void OnClear(object commandParameter)
        {
            ClearLogs();
        }

        private bool CanClear(object commandParameter)
        {
            return saving && Logs.Count > 0;
        }

        #endregion

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

        private void ClearLogsHandler(ClearLogsMessage message)
        {
            ClearLogs();
        }

        private void SaveLogHandler(SaveLogMessage message)
        {
            saving = false;

            Task.Run(() =>
            {
                var fileName = $"{Guid.NewGuid()}{FILE_EXTENSION}";
                var savePath = Path.Combine(message.SavePath, fileName);
                var fileStream = new FileStream(savePath, FileMode.Create);
                var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

                for (var i = 0; i < Logs.Count; i++)
                {
                    var saveData = $"{Logs[i].FileEvent,-10}{Logs[i].Timestamp,-10}{Logs[i].Path}";

                    streamWriter.WriteLine(saveData);
                }

                streamWriter.Close();
                fileStream.Close();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    saving = true;
                    RelayCommand.RaiseCanExecuteChanged();
                    DialogHelper.MessageBox(TITLE, $"{LABEL} {savePath}");
                });
            });
        }

        #endregion

        #region Other methods

        private void ClearLogs()
        {
            Logs.Clear();
            Messenger.Default.Send<ClearedLogsMessage>(new ClearedLogsMessage(false));
        }

        #endregion
    }
}
