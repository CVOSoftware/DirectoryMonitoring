namespace DirectoryMonitoring.Studio.Message
{
    internal class ClearedLogsMessage
    {
        public ClearedLogsMessage(bool informationToSave)
        {
            InformationToSave = informationToSave;
        }

        public bool InformationToSave { get;}
    }
}
