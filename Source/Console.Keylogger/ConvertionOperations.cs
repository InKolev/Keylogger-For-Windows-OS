using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Keylogger
{
    public static class ConvertionOperations
    {
        public static byte[] ImageToByteArray(Image image)
        { 
            var stream = new MemoryStream();
            var format = ImageFormat.Jpeg;
            image.Save(stream, format);

            return stream.ToArray();
        }

        public static Image ImageFromByteArray(byte[] imageAsByteArray)
        {
            var stream = new MemoryStream(imageAsByteArray);
            var image = Image.FromStream(stream);

            return image;
        }
    }
}
