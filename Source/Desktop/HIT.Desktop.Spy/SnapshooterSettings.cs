using System;

namespace HIT.Desktop.Spy
{
    [Serializable]
    public class SnapshooterSettings
    {
        private static SnapshooterSettings defaultSettings;

        static SnapshooterSettings()
        {
            defaultSettings = new SnapshooterSettings
            {
                SnapshotDelayInMilliseconds = 10000,
                HttpServiceSettings = HttpServiceSettings.DefaultSnapshooterSettings
            };
        }

        public int SnapshotDelayInMilliseconds { get; set; }

        public HttpServiceSettings HttpServiceSettings { get; set; }

        public static SnapshooterSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
