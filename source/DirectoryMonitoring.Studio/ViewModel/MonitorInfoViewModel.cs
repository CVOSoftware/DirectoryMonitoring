using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using LiveCharts;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;
using DirectoryMonitoring.Studio.Chart;
using System.Windows.Media;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class MonitorInfoViewModel : BaseViewModel
    {
        #region Const 

        private const string FILE_EXTENSION = ".txt";

        private const string TITLE = "Saving complete";

        private const string LABEL = "Saved to path:";

        private const string EVENT_CHANGE = "Changed";

        private const string EVENT_CREATE = "Created";

        private const string EVENT_DELETE = "Deleted";

        private const string EVENT_ERROR = "Error";

        private const string EVENT_RENAME = "Renamed";

        #endregion

        #region Field

        private bool scanComplete;

        private bool autoScroll;

        private bool saving;

        private Dictionary<string, ChartData> cache;

        #endregion

        #region Constructor

        public MonitorInfoViewModel()
        {
            Messenger.Default.Register<AddLogItemMessage>(this, AddLogItem);
            Messenger.Default.Register<NotifyScanCompleteMessage>(this, ScanCanceled);
            Messenger.Default.Register<ClearLogsMessage>(this, ClearLogsHandler);
            Messenger.Default.Register<SaveLogMessage>(this, SaveLogHandler);
            Messenger.Default.Register<UpdateLogChartsMessage>(this, UpdateLogCharts);

            InitializeCharts();

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

        public SeriesCollection Series { get; private set; }

        public ObservableCollection<LogViewModel> Logs { get; private set; }

        #endregion

        #region Command

        #region Clear

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

            if (cache.TryGetValue(message.FileEvent, out var data))
            {
                data.Count++;
            }
        }

        private void UpdateLogCharts(UpdateLogChartsMessage message)
        {
            foreach (var item in cache)
            {
                if(item.Value.Count > 0)
                {
                    item.Value.UpdateChart();
                }
            }
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

        private void InitializeCharts()
        {
            Series = new SeriesCollection();
            cache = new Dictionary<string, ChartData>();

            cache.Add(EVENT_CHANGE, new ChartData(Brushes.LightBlue));
            cache.Add(EVENT_CREATE, new ChartData(Brushes.Green));
            cache.Add(EVENT_DELETE, new ChartData(Brushes.Orange));
            cache.Add(EVENT_ERROR, new ChartData(Brushes.Red));
            cache.Add(EVENT_RENAME, new ChartData(Brushes.Violet));

            foreach(var item in cache)
            {
                Series.Add(item.Value.Series);
            }
        }

        private void ClearLogs()
        {
            Logs.Clear();
            ClearChartsCache();

            Messenger.Default.Send<ClearedLogsMessage>(new ClearedLogsMessage(false));
            Messenger.Default.Send<NotifyClearlogMessage>(NotifyClearlogMessage.Instance);
        }

        private void ClearChartsCache()
        {
            foreach (var item in cache)
            {
                item.Value.Count = 0;
                item.Value.Series.Values.Clear();
            }
        }

        #endregion
    }
}
