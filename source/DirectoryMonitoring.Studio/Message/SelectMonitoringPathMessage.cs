namespace DirectoryMonitoring.Studio.Message
{
    internal class SelectMonitoringPathMessage
    {
        public SelectMonitoringPathMessage(string selectedPath)
        {
            SelectedPath = selectedPath;
        }

        public string SelectedPath { get; }
    }
}
