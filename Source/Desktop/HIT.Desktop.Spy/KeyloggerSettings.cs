using System;

namespace HIT.Desktop.Spy
{
    [Serializable]
    public class KeyloggerSettings
    {
        private static KeyloggerSettings defaultSettings;

        static KeyloggerSettings()
        {
            defaultSettings = new KeyloggerSettings
            {
                RequestUrl = "http://localhost:62164/api/Sessions/PostKeysPressed",
                TimerSettings = TimerSettings.DefaultSettings
            };
        }

        public string RequestUrl { get; set; }

        public TimerSettings TimerSettings { get; set; }

        public static KeyloggerSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
