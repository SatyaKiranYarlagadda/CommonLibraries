namespace BackgroundProcessPoc
{
    public class BackgroundProcessConfig
    {
        public string Queue { get; set; }

        public int PollingIntervalInSeconds { get; set; }

        public int WorkerCount { get; set; }
    }
}
