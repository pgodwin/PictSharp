using System;
using System.IO;




namespace PictCodec
{
    /// <summary>
    /// Represents an image to consume 
    /// </summary>
    public class ImageDetails
    {

        /// <summary>
        /// Creates an image definition for 16bit, 24bit and 32-bit images
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bitsPerPixel"></param>
        /// <param name="channels"></param>
        /// <param name="pixelStream"></param>
        public ImageDetails(int width, int height, uint bitsPerPixel, uint channels, double horizontalRes, double verticalRes, Func<int, byte[]> getScanline)
        {
            this.Top = 0;
            this.Bottom = height;
            this.Left = 0;
            this.Right = width;
            this.BitsPerPixel = bitsPerPixel;
            this.Channels = channels;
            this.IsIndexed = false;
            this.Palette = null;
            
            this.HorizontalResolution = horizontalRes;
            this.VerticalResolution = verticalRes;

            this.GetScanline = getScanline;
        }

        public ImageDetails(int width, int height, uint bitsPerPixel, double horizontalRes, double verticalRes, PaletteEntry[] palette, Func<int, byte[]> getScanline)
        {
            this.Top = 0;
            this.Bottom = height;
            this.Left = 0;
            this.Right = width;
            this.BitsPerPixel = bitsPerPixel;
            this.Channels = 1; 
            this.Palette = palette;
            this.IsIndexed = true;
            this.HorizontalResolution = horizontalRes;
            this.VerticalResolution = verticalRes;

            this.GetScanline = getScanline;
        }

        public uint BitsPerPixel { get; }

        public uint Channels { get; }

        public bool IsIndexed { get; set; }

        public int Top { get; }

        public int Left { get; }

        public int Bottom { get;  }

        public int Right { get; }
        public double VerticalResolution { get; }
        public double HorizontalResolution { get; }

        public PaletteEntry[] Palette { get; }

        /// <summary>
        /// A method to return Scanlines
        /// </summary>
        public Func<int, byte[]> GetScanline { get; }
    }

}
