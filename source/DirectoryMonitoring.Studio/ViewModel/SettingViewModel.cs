﻿using System;
using System.IO;
using System.Windows;
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

        private const string ERROR_EVENT_TYPE = "Error";

        private const string ERROR_TITLE = "Watcher error";

        private const string ERROR_DESCRIPTION = "The specified path is not valid: ";

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

        private string monitoringPath;

        private string filter;

        private FileSystemWatcher watcher;

        #endregion

        #region Constructor

        public SettingViewModel()
        {
            Messenger.Default.Register<SelectMonitoringPathMessage>(this, SetSelectedPath);

            scanCanceled = true;
            isCreate = true;
            isDelete = true;
            filterFileName = true;
            filterDirectory = true;
            filterLastWrite = true;
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
                    SendLockSelectPath();
                    watcher = new FileSystemWatcher();
                }
                else
                {
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
