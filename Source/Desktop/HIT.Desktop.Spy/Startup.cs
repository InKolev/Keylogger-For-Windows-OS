using System;
using System.IO;
using System.Text;
using HIT.Common.Extensions;
using HIT.Common.Utils;
using HIT.Services;
using Newtonsoft.Json;

namespace HIT.Desktop.Spy
{
    class Startup
    {
        static void Main(string[] args)
        {
            // Load configs in a static class/dictionary
            var settingsPath = "InspectorSettingsNOTVALID.json"; // This path looks in the bin folder
            var settings = LoadSettings(settingsPath);
            var sessionId = ComposeSessionId(settings.SessionName);
            var httpService = new HttpService();
            var inspector = new Inspector(sessionId, settings, httpService);
            inspector.Start();
        }

        private static InspectorSettings LoadSettings(string inspectorSettingsFilePath)
        {
            if (!File.Exists(inspectorSettingsFilePath))
            {
                return InspectorSettings.DefaultSettings;
            }

            var inspectorSettingsFileContent = File.ReadAllText(inspectorSettingsFilePath);
            var inspectorSettings = JsonConvert.DeserializeObject<InspectorSettings>(inspectorSettingsFileContent);

            return inspectorSettings;
        }

        private static string ComposeSessionId(string sessionName)
        {
            ValidateSessionName(sessionName);

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
            // In which we store the session data
            var windowsFileNameMaxLength = 255;
            var sessionId = new StringBuilder(windowsFileNameMaxLength);
            sessionId.Append($"[{year}-{month}-{day}]")
                .Append("-")
                .Append($"[{hours}-{minutes}-{seconds}]")
                .Append("-")
                .Append($"[{sessionName}]");

            return sessionId.ToString();
        }

        /// <summary>
        /// Checks if the "sessionName" contains any characters 
        /// that are not allowed when creating files 
        /// directories in Windows OS. 
        /// Throws "ArgumentNullException" and "InvalidOperationException".
        /// </summary>
        /// <param name="sessionName">The string which needs to be validated.</param>
        private static void ValidateSessionName(string sessionName)
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
