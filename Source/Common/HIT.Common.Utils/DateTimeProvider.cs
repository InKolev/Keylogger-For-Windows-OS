using System;

namespace HIT.Common.Utils
{
    public abstract class DateTimeProvider
    {
        private static DateTimeProvider current = StandardDateTimeProvider.Instance;

        /// <summary>
        /// Represents the current instance of a datetime provider.
        /// Uses a StandardDateTimeProvider instance by default.
        /// </summary>
        public static DateTimeProvider Current
        {
            get
            {
                return current;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("The DateTimeProvider cannot be null");
                }

                current = value;
            }
        }

        public abstract DateTime UtcNow { get; }

        public abstract DateTime Now { get; }

        public static void ResetToDefault()
        {
            current = StandardDateTimeProvider.Instance;
        }
    }
}
