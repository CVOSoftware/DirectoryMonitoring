using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class SelectDirectoryViewModel : BaseViewModel
    {
        #region Fields

        private bool scanCanceled;

        private string monitoringPath;

        #endregion

        #region Constructors

        public SelectDirectoryViewModel()
        {
            Messenger.Default.Register<NotifySelectDirComponentMessage>(this, ScanEndNotify);

            scanCanceled = true;
        }

        #endregion

        #region Properties

        public bool ScanCanceled
        {
            get => scanCanceled;
            set => SetValue(ref scanCanceled, value);
        }

        public string MonitoringPath
        {
            get => monitoringPath;
            set => SetValue(ref monitoringPath, value);
        }

        #endregion

        #region Coomands

        #region SelectMonitorDirectory

        private RelayCommand selectMonitorDirectory;

        public RelayCommand SelectMonitorDirectory => RelayCommand.Register(ref selectMonitorDirectory, OnSelect, CanSelect);

        private void OnSelect(object commandParameter)
        {
            MonitoringPath = FileSystemDialogHelper.GetDirectory();
        }

        private bool CanSelect(object commandParameter)
        {
            return scanCanceled;
        }

        #endregion

        #endregion

        #region Message handlers

        private void ScanEndNotify(NotifySelectDirComponentMessage message)
        {
            ScanCanceled = message.ScanCanceled;
        }

        #endregion
    }
}
