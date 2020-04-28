namespace DirectoryMonitoring.Studio.Message
{
    internal class NotifyMonitorInfoMessage
    {
        public NotifyMonitorInfoMessage(bool scanComplete)
        {
            ScanComplete = scanComplete;
        }

        public bool ScanComplete { get; }
    }
}
