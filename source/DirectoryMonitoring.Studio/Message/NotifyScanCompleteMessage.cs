namespace DirectoryMonitoring.Studio.Message
{
    internal class NotifyScanCompleteMessage
    {
        public NotifyScanCompleteMessage(bool scanCanceled)
        {
            ScanCanceled = scanCanceled;
        }

        public bool ScanCanceled { get; }
    }
}
