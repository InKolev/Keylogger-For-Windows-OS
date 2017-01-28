using System;

namespace HIT.Desktop.Spy
{
    [Serializable]
    public class HttpServiceSettings
    {
        private static HttpServiceSettings defaultSnapshooterSettings;
        private static HttpServiceSettings defaultKeyloggerSettings;

        static HttpServiceSettings()
        {
            var defaultBaseAddress = "http://localhost:62164/";

            defaultSnapshooterSettings = new HttpServiceSettings
            {
                BaseAddress = defaultBaseAddress,
                RequestURL = "api/Sessions/PostSnapshot"
            };

            defaultKeyloggerSettings = new HttpServiceSettings
            {
                BaseAddress = defaultBaseAddress,
                RequestURL = "api/Sessions/PostKeysPressed"
            };
        }

        public string BaseAddress { get; set; }

        public string RequestURL { get; set; }

        public static HttpServiceSettings DefaultSnapshooterSettings
        {
            get
            {
                return defaultSnapshooterSettings;
            }
        }

        public static HttpServiceSettings DefaultKeyloggerSettings
        {
            get
            {
                return defaultKeyloggerSettings;
            }
        }
    }
}
