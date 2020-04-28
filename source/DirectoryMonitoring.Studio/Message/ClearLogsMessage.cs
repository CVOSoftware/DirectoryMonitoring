namespace DirectoryMonitoring.Studio.Message
{
    internal class ClearLogsMessage
    {
        private static ClearLogsMessage single = new ClearLogsMessage();

        public static ClearLogsMessage Instance => single;
    }
}
