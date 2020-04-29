using System;
using System.IO;
using System.Windows;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class SettingViewModel : BaseViewModel
    {
        #region Const

        private const bool SCAN_START = true;

        private const bool SCAN_PAUSE = false;

        private const string ERROR_EVENT_TYPE = "Error";

        #endregion

        #region Fields

        private bool scanCanceled;

        private bool includeSubdirectories;

        private bool isChange;

        private bool isCreate;

        private bool isDelete;

        private bool isError;

        private bool isRename;

        private string monitoringPath;

        private FileSystemWatcher watcher;

        #endregion

        #region Constructor

        public SettingViewModel()
        {
            Messenger.Default.Register<SelectMonitoringPathMessage>(this, SetSelectedPath);

            scanCanceled = true;
            isCreate = true;
            isDelete = true;
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

        #endregion

        #region Command

        #region Start

        private RelayCommand start;

        public RelayCommand Start => RelayCommand.Register(ref start, OnStart, CanStart);

        private void OnStart(object commandParameter)
        {
            ScanCanceled = false;

            if (watcher == null)
            {
                SendLockSelectPath();
                watcher = new FileSystemWatcher();
                watcher.Path = monitoringPath;
            }
            else
            {
                EventStackDedubscribe();
            }
            
            try
            {
                watcher.EnableRaisingEvents = SCAN_START;
                watcher.IncludeSubdirectories = IncludeSubdirectories;
                EventStackSubscribe();
            }
            catch
            {
                // Show dialog message
            }
        }

        private bool CanStart(object commandParameter)
        {
            return scanCanceled 
                   && monitoringPath.Equals(string.Empty) == false
                   && CheckSelectedEvent();
        }

        #endregion

        #region Pause

        private RelayCommand pause;

        public RelayCommand Pause => RelayCommand.Register(ref pause, OnPause, CanPause);

        private void OnPause(object commandParameter)
        {
            ScanCanceled = true;
            watcher.EnableRaisingEvents = SCAN_PAUSE;
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
            ScanCanceled = true;
            SendLockSelectPath();
            EventStackDedubscribe();
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

        #endregion

        #region Other methods

        private void SendLockSelectPath()
        {
            var message = new NotifyScanCompleteMessage(ScanCanceled);

            Messenger.Default.Send<NotifyScanCompleteMessage>(message);
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
            var message = new AddLogItemMessage(fileEvent, path, timestamp);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Messenger.Default.Send<AddLogItemMessage>(message);
            });
        }

        #endregion
    }
}
