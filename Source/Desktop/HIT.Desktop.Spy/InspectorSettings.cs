using System;

namespace HIT.Desktop.Spy
{
    [Serializable]
    public class InspectorSettings
    {
        private static InspectorSettings defaultSettings;

        static InspectorSettings()
        {
            defaultSettings = new InspectorSettings
            {
                SessionName = Guid.NewGuid().ToString(),
                KeyloggerSettings = KeyloggerSettings.DefaultSettings,
                SnapshooterSettings = SnapshooterSettings.DefaultSettings
            };
        }

        public string SessionName { get; set; }

        public KeyloggerSettings KeyloggerSettings { get; set; }

        public SnapshooterSettings SnapshooterSettings { get; set; }

        public static InspectorSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
