using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIT.Desktop.Spy.Offline
{
    public class Snapshooter : IRunnable
    {
        private bool isRunning = true;

        public Task Start()
        {
            var primaryScreenBounds = Screen.PrimaryScreen.Bounds;
            var primaryScreenWidth = primaryScreenBounds.Width;
            var primaryScreenHeight = primaryScreenBounds.Height;

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

                        Thread.Sleep(10000);
                    }
                }
            }

            return null;
        }

        public void Stop()
        {
            this.isRunning = false;
        }
    }
}
