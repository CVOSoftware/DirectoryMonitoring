namespace DirectoryMonitoring.Studio.Message
{
    internal class NotifySelectDirComponentMessage
    {
        public NotifySelectDirComponentMessage(bool scanCanceled)
        {
            ScanCanceled = scanCanceled;
        }

        public bool ScanCanceled { get; }
    }
}
