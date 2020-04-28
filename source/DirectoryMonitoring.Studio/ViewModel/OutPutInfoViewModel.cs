using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MVVMLight.Messaging;
using DirectoryMonitoring.Studio.Base;
using DirectoryMonitoring.Studio.Message;
using DirectoryMonitoring.Studio.Helper;

namespace DirectoryMonitoring.Studio.ViewModel
{
    internal class OutputInfoViewModel : BaseViewModel
    {
        #region Const

        private const string DEFAULT_SAVE_DIRECTORY_NAME = "Logs";

        #endregion

        #region Fields

        private bool saving;

        private bool scanCompleted;

        private bool informationToSave;

        private string saveLogPath;

        private string cacheSaveLogPath;

        #endregion

        #region Constructor

        public OutputInfoViewModel()
        {
            Messenger.Default.Register<NotifyOutputInfoComponentMessage>(this, ScanEndNotify);
            Messenger.Default.Register<ClearedLogsMessage>(this, ClearedLogs);

            saveLogPath = CreateDefaultSaveLogDirectoryPath();
            cacheSaveLogPath = saveLogPath;
        }

        #endregion

        #region Properties

        public bool ScanCompleted
        {
            get => scanCompleted;
            set => SetValue(ref scanCompleted, value);
        }

        public bool InformationToSave
        {
            get => informationToSave;
            set => SetValue(ref informationToSave, value);
        }

        public string SaveLogPath
        {
            get => saveLogPath;
            set => SetValue(ref saveLogPath, value);
        }

        #endregion

        #region Commands

        #region OpenDirectoryCommand

        private RelayCommand openDirectory;

        public RelayCommand OpenDirectory => RelayCommand.Register(ref openDirectory, OnOpenDirectory);

        private void OnOpenDirectory(object comandParameter)
        {
            Task.Run(OpenFileExplorer);
        }

        #endregion

        #region SaveLogCommand

        private RelayCommand saveLog;

        public RelayCommand SaveLog => RelayCommand.Register(ref saveLog, OnSaveLog, CanSaveLog);

        private void OnSaveLog(object comandParameter)
        {

        }

        private bool CanSaveLog(object commandParameters)
        {
            return scanCompleted && informationToSave;
        }

        #endregion

        #region SetDefaultSavePath

        private RelayCommand setDefaultSavePath;

        public RelayCommand SetDefaultSavePath => RelayCommand.Register(ref setDefaultSavePath, OnSetDefault, CanSetDefault);

        private void OnSetDefault(object commandParameter)
        {
            SaveLogPath = cacheSaveLogPath;
        }

        private bool CanSetDefault(object coomandParameter)
        {
            return saveLogPath.Equals(cacheSaveLogPath, StringComparison.OrdinalIgnoreCase) == false
                   && saving == false;
        }

        #endregion

        #region SelectDirectoryCommand

        private RelayCommand selectDirectory;

        public RelayCommand SelectDirectory => RelayCommand.Register(ref selectDirectory, OnSelectDirectory);

        private void OnSelectDirectory(object comandParameter)
        {
            var result = FileSystemDialogHelper.GetDirectory();

            if (result != string.Empty && result != saveLogPath)
            {
                SaveLogPath = result;
            }
        }

        #endregion

        #endregion

        #region Message handlers

        private void ScanEndNotify(NotifyOutputInfoComponentMessage message)
        {
            ScanCompleted = message.ScanCompleted;
            InformationToSave = message.InformationToSave;
        }

        private void ClearedLogs(ClearedLogsMessage message)
        {
            informationToSave = message.InformationToSave;
        }

        #endregion

        #region Other methods

        private string CreateDefaultSaveLogDirectoryPath()
        {
            var assemblyRootPath = AssemblyHelper.GetAssemblyRootPath<OutputInfoViewModel>();
            var defaultSaveLogDirectoryPath = Path.Combine(assemblyRootPath, DEFAULT_SAVE_DIRECTORY_NAME);

            return defaultSaveLogDirectoryPath;
        }

        private void OpenFileExplorer()
        {
            if (Directory.Exists(saveLogPath) == false)
            {
                Directory.CreateDirectory(saveLogPath);
            }

            FileExplorerHelper.OpenFileExplorer(saveLogPath);
        }

        #endregion
    }
}
