using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class SettingViewModel : BaseViewModel
    {
        #region Const

        private const bool SCAN_START = true;

        private const bool SCAN_PAUSE = false;

        private const int LIMIT_LOG_COUNT = 5000;

        private const int HOUR_LIMIT = 1;

        private const int MINUTE_LIMIT = 0;

        private const int SECOND_LIMIT = 0;

        private const int TIMER_SECOND = 1;

        private const string DATETIME_FORMAT = @"hh\:mm\:ss";

        private const string DEFAULT_TIMER = "00:00:00";

        private const string ERROR_EVENT_TYPE = "Error";

        private const string ERROR_TITLE = "Watcher error";

        private const string ERROR_DESCRIPTION = "The specified path is not valid: ";

        private const string LIMIT_TITLE = "Monitoring completed";

        private const string TIME_LIMIT_MSG_PART_1 = "Monitoring is completed because the execution time has exceeded ";

        private const string TIME_LIMIT_MSG_PART_2 = " hours";

        private const string LOG_LIMIT_MSG_PART_1 = "Monitoring is completed because the log limit of ";

        private const string LOG_LIMIT_MSG_PART_2 = " has been reached";

        #endregion

        #region Fields

        private bool scanCanceled;

        private bool includeSubdirectories;

        private bool isChange;

        private bool isCreate;

        private bool isDelete;

        private bool isError;

        private bool isRename;

        private bool filterAttribute;

        private bool filterDirectory;

        private bool filterLastAccess;

        private bool filterSecurity;

        private bool filterCreationTime;

        private bool filterFileName;

        private bool filterLastWrite;

        private bool filterSize;

        private int logCount;

        private string monitoringPath;

        private string filter;

        private string watch;

        private double counterMiliseconds;

        private TimeSpan perfomanceLimit;

        private EventHandler timerEventHandler;

        private DispatcherTimer timer;

        private FileSystemWatcher watcher;

        #endregion

        #region Constructor

        public SettingViewModel()
        {
            Messenger.Default.Register<SelectMonitoringPathMessage>(this, SetSelectedPath);
            Messenger.Default.Register<NotifyClearlogMessage>(this, ClearLogs);

            scanCanceled = true;
            isCreate = true;
            isDelete = true;
            filterFileName = true;
            filterDirectory = true;
            filterLastWrite = true;
            watch = DEFAULT_TIMER;
            perfomanceLimit = new TimeSpan(HOUR_LIMIT, MINUTE_LIMIT, SECOND_LIMIT);
            monitoringPath = string.Empty;
        }

        #endregion

        #region Properties

        public bool ScanCanceled
        {
            get => scanCanceled;
            set => SetValue(ref scanCanceled, value);
        }

        public bool IncludeSubdirectories
        {
            get => includeSubdirectories;
            set => SetValue(ref includeSubdirectories, value);
        }

        public bool IsChange
        {
            get => isChange;
            set => SetValue(ref isChange, value);
        }

        public bool IsCreate
        {
            get => isCreate;
            set => SetValue(ref isCreate, value);
        }

        public bool IsDelete
        {
            get => isDelete;
            set => SetValue(ref isDelete, value);
        }

        public bool IsError
        {
            get => isError;
            set => SetValue(ref isError, value);
        }

        public bool IsRename
        {
            get => isRename;
            set => SetValue(ref isRename, value);
        }

        public bool FilterAttribute
        {
            get => filterAttribute;
            set => SetValue(ref filterAttribute, value);
        }

        public bool FilterDirectory
        {
            get => filterDirectory;
            set => SetValue(ref filterDirectory, value);
        }

        public bool FilterLastAccess
        {
            get => filterLastAccess;
            set => SetValue(ref filterLastAccess, value);
        }

        public bool FilterSecurity
        {
            get => filterSecurity;
            set => SetValue(ref filterSecurity, value);
        }

        public bool FilterCreationTime
        {
            get => filterCreationTime;
            set => SetValue(ref filterCreationTime, value);
        }

        public bool FilterFileName
        {
            get => filterFileName;
            set => SetValue(ref filterFileName, value);
        }

        public bool FilterLastWrite
        {
            get => filterLastWrite;
            set => SetValue(ref filterLastWrite, value);
        }

        public bool FilterSize
        {
            get => filterSize;
            set => SetValue(ref filterSize, value);
        }

        public string Watch
        {
            get => watch;
            set => SetValue(ref watch, value);
        }

        public string Filter
        {
            get => filter;
            set => SetValue(ref filter, value);
        }

        #endregion

        #region Command

        #region Start

        private RelayCommand start;

        public RelayCommand Start => RelayCommand.Register(ref start, OnStart, CanStart);

        private void OnStart(object commandParameter)
        {
            ScanCanceled = false;

            try
            {
                if (watcher == null)
                {
                    StartTimer();
                    SendLockSelectPath();
                    Messenger.Default.Send<ClearLogsMessage>(ClearLogsMessage.Instance);
                    watcher = new FileSystemWatcher();
                }
                else
                {
                    ContinueTimer();
                    EventStackDedubscribe();
                }

                EventStackSubscribe();
                SetWatcherNotifyFilters();
                watcher.Filter = Filter;
                watcher.Path = monitoringPath;
                watcher.IncludeSubdirectories = IncludeSubdirectories;
                watcher.EnableRaisingEvents = SCAN_START;
            }
            catch
            {
                var description = $"{ERROR_DESCRIPTION}{monitoringPath}";

                StopTimer();
                DialogHelper.MessageBox(ERROR_TITLE, description);
                ScanCanceled = true;
                monitoringPath = string.Empty;
                SendLockSelectPath();
                SendResetSelectedMonitoringPath();
                watcher.Dispose();
                watcher = null;
            }
        }

        private bool CanStart(object commandParameter)
        {
            return scanCanceled 
                   && monitoringPath.Equals(string.Empty) == false
                   && CheckSelectedEvent()
                   && CheckSelectedNotifyFilter();
        }

        #endregion

        #region Pause

        private RelayCommand pause;

        public RelayCommand Pause => RelayCommand.Register(ref pause, OnPause, CanPause);

        private void OnPause(object commandParameter)
        {
            ScanCanceled = true;
            watcher.EnableRaisingEvents = SCAN_PAUSE;
            PauseTimer();
        }

        private bool CanPause(object commandParameter)
        {
            return scanCanceled == false;
        }

        #endregion

        #region Stop

        private RelayCommand stop;

        public RelayCommand Stop => RelayCommand.Register(ref stop, OnStop, CanStop);

        private void OnStop(object commandParameter)
        {
            StopTimer();
            ScanCanceled = true;
            SendLockSelectPath();
            EventStackDedubscribe();
            logCount = 0;
            watcher.EnableRaisingEvents = SCAN_PAUSE;
            watcher.Dispose();
            watcher = null;
            
        }

        private bool CanStop(object commandParameter)
        {
            return watcher != null;
        }

        #endregion

        #endregion

        #region Message handlers

        private void SetSelectedPath(SelectMonitoringPathMessage message)
        {
            monitoringPath = message.SelectedPath;
        }

        private void ClearLogs(NotifyClearlogMessage message)
        {
            logCount = 0;
        }

        #endregion

        #region Other methods

        private void SendLockSelectPath()
        {
            var message = new NotifyScanCompleteMessage(scanCanceled);

            Messenger.Default.Send<NotifyScanCompleteMessage>(message);
        }

        private void SendResetSelectedMonitoringPath()
        {
            var message = new ResetSelectedPathMessage(monitoringPath);

            Messenger.Default.Send<ResetSelectedPathMessage>(message);
        }

        private void SetWatcherNotifyFilters()
        {
            watcher.NotifyFilter = default;

            if (filterAttribute) watcher.NotifyFilter |= NotifyFilters.Attributes;
            if (filterDirectory) watcher.NotifyFilter |= NotifyFilters.DirectoryName;
            if (filterLastAccess) watcher.NotifyFilter |= NotifyFilters.LastAccess;
            if (filterSecurity) watcher.NotifyFilter |= NotifyFilters.Security;
            if (filterCreationTime) watcher.NotifyFilter |= NotifyFilters.CreationTime;
            if (filterFileName) watcher.NotifyFilter |= NotifyFilters.FileName;
            if (filterLastWrite) watcher.NotifyFilter |= NotifyFilters.LastWrite;
            if (filterSize) watcher.NotifyFilter |= NotifyFilters.Size;
        }

        private void EventStackSubscribe()
        {
            if (isChange) watcher.Changed += DirectoryEventHandler;
            if (isCreate) watcher.Created += DirectoryEventHandler;
            if (isDelete) watcher.Deleted += DirectoryEventHandler;
            if (isError) watcher.Error += DirectoryEventHandler;
            if (isRename) watcher.Renamed += DirectoryEventHandler;
        }

        private void EventStackDedubscribe()
        {
            watcher.Changed -= DirectoryEventHandler;
            watcher.Created -= DirectoryEventHandler;
            watcher.Deleted -= DirectoryEventHandler;
            watcher.Error -= DirectoryEventHandler;
            watcher.Renamed -= DirectoryEventHandler;
        }

        private bool CheckSelectedEvent()
        {
            return isChange
                   || isCreate
                   || isDelete
                   || isError
                   || isRename;
        }

        private bool CheckSelectedNotifyFilter()
        {
            return filterAttribute
                   || filterDirectory
                   || filterCreationTime
                   || filterFileName
                   || filterLastAccess
                   || filterLastWrite
                   || filterSecurity
                   || filterSize;
        }

        private void DirectoryEventHandler(object sender, FileSystemEventArgs e)
        {
            SendLogToSave(e.ChangeType.ToString(), e.FullPath, DateTime.Now);
        }

        private void DirectoryEventHandler(object sender, ErrorEventArgs e)
        {
            SendLogToSave(ERROR_EVENT_TYPE, monitoringPath, DateTime.Now);
        }

        private void SendLogToSave(string fileEvent, string path, DateTime timestamp)
        {
            if (logCount >= LIMIT_LOG_COUNT)
            {
                var dialogMessage = $"{LOG_LIMIT_MSG_PART_1}{LIMIT_LOG_COUNT}{LOG_LIMIT_MSG_PART_2}";

                OnStop(this);
                RelayCommand.RaiseCanExecuteChanged();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DialogHelper.MessageBox(LIMIT_TITLE, dialogMessage);
                });

                return;
            }

            var message = new AddLogItemMessage(fileEvent, path, timestamp);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Messenger.Default.Send<AddLogItemMessage>(message);
            });

            logCount++;
        }

        private void StartTimer()
        {
            counterMiliseconds = 0;
            timerEventHandler = new EventHandler(TimerTick);
            timer = new DispatcherTimer();
            timer.Tick += timerEventHandler;
            timer.Interval = TimeSpan.FromSeconds(TIMER_SECOND);
            timer.Start();
        }

        private void ContinueTimer()
        {
            timer.Start();
        }

        private void PauseTimer()
        {
            timer.Stop();
        }

        private void StopTimer()
        {
            timer.Stop();
            timer.Tick -= timerEventHandler;
            Watch = DEFAULT_TIMER;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            counterMiliseconds++;

            var timeSpan = TimeSpan.FromSeconds(counterMiliseconds);

            if(timeSpan >= perfomanceLimit)
            {
                var dialogMessage = $"{TIME_LIMIT_MSG_PART_1}{HOUR_LIMIT}{TIME_LIMIT_MSG_PART_2}";

                OnStop(sender);
                RelayCommand.RaiseCanExecuteChanged();
                DialogHelper.MessageBox(LIMIT_TITLE, dialogMessage);
                
                return;
            }

            Messenger.Default.Send<UpdateLogChartsMessage>(new UpdateLogChartsMessage());

            Watch = timeSpan.ToString(DATETIME_FORMAT);
        }

        #endregion
    }
}
