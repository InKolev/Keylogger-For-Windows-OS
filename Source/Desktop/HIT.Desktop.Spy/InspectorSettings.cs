using System;

namespace HIT.Desktop.Spy
{
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
