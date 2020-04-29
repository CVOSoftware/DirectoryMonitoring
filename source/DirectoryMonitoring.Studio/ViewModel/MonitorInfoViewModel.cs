﻿using System.Collections.ObjectModel;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;

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
            Messenger.Default.Register<ClearLogsMessage>(this, ClearLogsHandler);
            Messenger.Default.Register<SaveLogMessage>(this, SaveLogHandler);

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
            return Logs.Count > 0;
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
            DialogHelper.SaveLog(message.SavePath, Logs);
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
