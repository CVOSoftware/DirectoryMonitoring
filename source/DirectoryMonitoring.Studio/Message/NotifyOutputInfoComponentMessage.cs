namespace DirectoryMonitoring.Studio.Message
{
    internal class NotifyOutputInfoComponentMessage
    {
        public NotifyOutputInfoComponentMessage(bool scanCompleted, bool informationToSave)
        {
            ScanCompleted = scanCompleted;
            InformationToSave = informationToSave;
        }

        public bool ScanCompleted { get;}

        public bool InformationToSave { get; }
    }
}
