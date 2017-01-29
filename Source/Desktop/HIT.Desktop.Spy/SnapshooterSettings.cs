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
                RequestUrl = "http://localhost:62164/api/Sessions/PostSnapshot"
            };
        }

        public string RequestUrl { get; set; }

        public int SnapshotDelayInMilliseconds { get; set; }

        public static SnapshooterSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
