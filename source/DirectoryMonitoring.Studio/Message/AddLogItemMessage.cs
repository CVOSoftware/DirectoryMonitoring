using System;

namespace DirectoryMonitoring.Studio.Message
{
    internal class AddLogItemMessage
    {
        #region Comstructor

        public AddLogItemMessage(string fileEvent, string path, DateTime timestamp)
        {
            FileEvent = fileEvent;
            Path = path;
            Timestamp = timestamp;
        }

        #endregion

        #region Properties

        public string FileEvent { get; }

        public string Path { get; }

        public DateTime Timestamp { get; }

        #endregion
    }
}
