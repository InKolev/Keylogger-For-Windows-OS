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
                KeysPressedThreshold = 20,
                HttpServiceSettings = HttpServiceSettings.DefaultKeyloggerSettings,
                TimerSettings = TimerSettings.DefaultSettings
            };
        }

        public int KeysPressedThreshold { get; set; }

        public TimerSettings TimerSettings { get; set; }

        public HttpServiceSettings HttpServiceSettings { get; set; }

        public static KeyloggerSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
