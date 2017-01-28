using System;

namespace HIT.Desktop.Spy
{
    [Serializable]
    public class TimerSettings
    {
        private static TimerSettings defaultSettings;

        static TimerSettings()
        {
            defaultSettings = new TimerSettings
            {
                DueTimeInMilliseconds = 2000,
                PeriodInMilliseconds = 10000,
                ObjectState = default(object)
            };
        }

        public object ObjectState { get; set; }

        public int DueTimeInMilliseconds { get; set; }

        public int PeriodInMilliseconds { get; set; }

        public static TimerSettings DefaultSettings
        {
            get
            {
                return defaultSettings;
            }
        }
    }
}
