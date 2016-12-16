using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.Common.Utils
{
    public abstract class DateTimeProvider
    {
        private static DateTimeProvider current = DefaultDateTimeProvider.Instance;

        public static DateTimeProvider Current
        {
            get
            {
                return DateTimeProvider.current;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("The DateTimeProvider cannot be null");
                }

                DateTimeProvider.current = value;
            }
        }

        public abstract DateTime UtcNow { get; }

        public abstract DateTime Now { get; }

        public static void ResetToDefault()
        {
            DateTimeProvider.current = DefaultDateTimeProvider.Instance;
        }
    }

    // TODO: Make it singleton with concurrent access considerations
    public class DefaultDateTimeProvider : DateTimeProvider
    {
        public static readonly DateTimeProvider Instance = new DefaultDateTimeProvider();

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
