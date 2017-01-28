using System;
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
    public class Snapshooter : IDataCollector
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
            var destinationX = 0;
            var destinationY = 0;

            using (var bitmapScreenCapture = new Bitmap(primaryScreenWidth, primaryScreenHeight))
            {
                using (var graphics = Graphics.FromImage(bitmapScreenCapture))
                {
                    while (this.isRunning)
                    {
                        graphics.CopyFromScreen(
                            primaryScreenBounds.X,
                            primaryScreenBounds.Y,
                            destinationX,
                            destinationY,
                            bitmapScreenCapture.Size,
                            CopyPixelOperation.SourceCopy);

                        var imageAsByteArray = ConvertionOperations.ImageToByteArray(bitmapScreenCapture);

                        try
                        {
                            var baseAddress = "http://localhost:62164/";
                            var requestUri = "api/Sessions/PostSnapshot";
                            var model = new SnapshotViewModel
                            {
                                SessionId = this.SessionId,
                                SnapshotAsByteArray = imageAsByteArray
                            };

                            await SendTo(baseAddress,requestUri, model);
                        }
                        catch (Exception exc)
                        {
                            // TODO: Log the exception cause
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
