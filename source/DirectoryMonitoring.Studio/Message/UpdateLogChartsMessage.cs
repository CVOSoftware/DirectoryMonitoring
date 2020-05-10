namespace DirectoryMonitoring.Studio.Message
{
    internal class UpdateLogChartsMessage
    {
        private static UpdateLogChartsMessage instance = new UpdateLogChartsMessage();

        public static UpdateLogChartsMessage Instance => instance;
    }
}
