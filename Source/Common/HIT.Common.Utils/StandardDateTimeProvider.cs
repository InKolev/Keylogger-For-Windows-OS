using System;

namespace HIT.Common.Utils
{
    public sealed class StandardDateTimeProvider : DateTimeProvider
    {
        private static readonly StandardDateTimeProvider instance = new StandardDateTimeProvider();

        // Explicit static constructor
        // In order to prevent the compiler from 
        // Marking the field with the "beforefieldinit" flag
        static StandardDateTimeProvider() { }

        private StandardDateTimeProvider() { }

        public static DateTimeProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public override DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public override DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
