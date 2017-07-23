using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
{
    public static class BitmapExtensions
    {
        public static byte[] ToByteArray(this Bitmap bitmap)
        {

            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }

        public static int[] ToIntArray(this Bitmap bmp)
        {
            var bytes = bmp.ToByteArray();
            var size = bytes.Count() / sizeof(int);
            var ints = new int[size];
            for (var index = 0; index < size; index++)
            {
                ints[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }
            return ints;
        }

        public static ImageDetails GetImageDetails(this Bitmap bmp)
        {
            return new ImageDetails(bmp);
        }

    }

    public class ImageDetails
    {
        public ImageDetails(Image image)
        {
            this.BitsPerPixel = Image.GetPixelFormatSize(image.PixelFormat);
            
            this.IsIndexed = image.PixelFormat == PixelFormat.Format1bppIndexed ||
                                image.PixelFormat == PixelFormat.Format4bppIndexed ||
                                image.PixelFormat == PixelFormat.Format8bppIndexed;

            this.Channels = IsIndexed ? 1 : 4;

            this.Top = 0;
            this.Left = 0;
            this.Bottom = image.Height;
            this.Right = image.Width;
        }

        public int BitsPerPixel { get; set; }

        public int Channels { get; set; }

        public bool IsIndexed { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public int Bottom { get; set; }

        public int Right { get; set; }

        
    }

}
