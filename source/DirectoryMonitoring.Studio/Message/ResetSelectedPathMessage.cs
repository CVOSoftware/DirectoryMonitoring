namespace DirectoryMonitoring.Studio.Message
{
    internal class ResetSelectedPathMessage
    {
        public ResetSelectedPathMessage(string monitoringPath)
        {
            MonitoringPath = monitoringPath;
        }

        public string MonitoringPath { get; }
    }
}
