using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PictSharp.Drawing
{
    /// <summary>
    /// Extensions to System.Drawing.Bitmap to support PICT Encoding.
    /// </summary>
    public static class BitmapExtensions
    {
        internal static byte[] ToByteArray(this Bitmap bitmap)
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

        internal static int[] ToIntArray(this Bitmap bmp)
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

        /// <summary>
        /// Get's a row of pixels from the image.
        /// </summary>
        /// <param name="imageDetails">ImageDetails containing the meta data, etc for the image</param>
        /// <param name="pixels">byte array representing the full image</param>
        /// <param name="y">The row to return</param>
        /// <returns></returns>
        private static byte[] GetScanLine(ImageDetails imageDetails, byte[] pixels, int y)
        {
            var h = imageDetails.Bottom;
            var stride = pixels.Length / h;
            byte[] scanlineBytes = new byte[stride];
            int offset = y * stride;
            for (int x = 0; x < stride; x++)
            {
                scanlineBytes[x] = pixels[x + offset];
            }

            return scanlineBytes;
        }

        /// <summary>
        /// Converts a System.Drawing.Bitmap to an ImageDetails object
        /// </summary>
        /// <param name="image">System.Drawing.Bitmap to convert</param>
        /// <returns>ImageDetails representation of the bitmap.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ImageDetails ToImageDetails(this Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            ImageDetails details = default;
            var indexed = image.PixelFormat == PixelFormat.Format1bppIndexed ||
                                image.PixelFormat == PixelFormat.Format4bppIndexed ||
                                image.PixelFormat == PixelFormat.Format8bppIndexed;

            var bpp = Image.GetPixelFormatSize(image.PixelFormat);
            var bytes = image.ToByteArray();

            var channels = indexed ? 1 : bpp / 8;

            var getRow = new Func<int, byte[]>((y) => GetScanLine(details, bytes, y));


            if (!indexed)
            {
                details = new ImageDetails(image.Width, image.Height, (uint)bpp, (uint)channels, image.HorizontalResolution, image.VerticalResolution, getRow);
            }
            else
            {
                var palette = image.Palette.Entries.Select(e => new PaletteEntry(e.A, e.R, e.G, e.B)).ToArray();
                details = new ImageDetails(image.Width, image.Height, (uint)bpp, image.HorizontalResolution, image.VerticalResolution, palette, getRow);
            }

            return details; 
        }


        /// <inheritdoc/>
        public static void SaveAsPict(this Bitmap bitmap, Stream output)
        {
            var image = bitmap.ToImageDetails();
            var encoder = new Pict();
            encoder.Encode(output, image);
        }

        /// <summary>
        /// Saves the Bitmap as a PICT file to the specified file
        /// </summary>
        /// <param name="bitmap">Source bitmap</param>
        /// <param name="filename">Output filename. If the File already exists, an IOException is throw</param>
        public static void SaveAsPict(this Bitmap bitmap, string filename)
        {
            using (var stream = new FileStream(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                SaveAsPict(bitmap, stream);
        }

    }

}

