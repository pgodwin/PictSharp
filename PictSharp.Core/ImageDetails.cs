using System;

namespace PictSharp
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
        /// <param name="horizontalRes"></param>
        /// <param name="verticalRes"></param>
        /// <param name="getScanline"></param>
        public ImageDetails(int width, int height, uint bitsPerPixel, uint channels, double horizontalRes, double verticalRes, Func<int, byte[]> getScanline)
        {
            this.Top = 0;
            this.Bottom = height;
            this.Left = 0;
            this.Right = width;

            this.Width = width;
            this.Height = height;

            this.BitsPerPixel = bitsPerPixel;
            this.Channels = channels;
            this.IsIndexed = false;
            this.Palette = null;

            this.HorizontalResolution = (ushort)Math.Floor(horizontalRes + 0.5);
            this.VerticalResolution = (ushort)Math.Floor(verticalRes + 0.5);

            this.GetScanline = getScanline;
        }

        /// <summary>
        /// Creates an ImageDetails definition for Indexed Images (1bpp, 2bpp, 4bpp, 8bpp images)
        /// </summary>
        /// <param name="width">Image Width in Pixels</param>
        /// <param name="height">Image height in Pixels</param>
        /// <param name="bitsPerPixel">Bits Per Pixel</param>
        /// <param name="horizontalRes">Horizontal Resolution in Pixels Per Inch</param>
        /// <param name="verticalRes">Vertical Resolution in Pixels per Inch</param>
        /// <param name="palette">Palette for the image</param>
        /// <param name="getScanline">Callback to return a scan line of the source image</param>
        public ImageDetails(int width, int height, uint bitsPerPixel, double horizontalRes, double verticalRes, PaletteEntry[] palette, Func<int, byte[]> getScanline)
        {
            this.Top = 0;
            this.Bottom = height;
            this.Left = 0;
            this.Right = width;
            this.Width = width;
            this.Height = height;
            this.BitsPerPixel = bitsPerPixel;
            this.Channels = 1; 
            this.Palette = palette;
            this.IsIndexed = true;

            

            this.HorizontalResolution = (ushort)Math.Floor(horizontalRes + 0.5);
            this.VerticalResolution = (ushort)Math.Floor(verticalRes + 0.5); 

            this.GetScanline = getScanline;
        }

        /// <summary>
        /// Bits per pixel
        /// </summary>
        public uint BitsPerPixel { get; }

        /// <summary>
        /// Number of channels
        /// </summary>
        public uint Channels { get; }

        /// <summary>
        /// Is the image an Indexed image (ie Bpp less than 16)
        /// </summary>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Y Position of the start of the image frame
        /// </summary>
        public int Top { get; }

        /// <summary>
        /// X Position of the start of the image frame
        /// </summary>
        public int Left { get; }

        /// <summary>
        /// Y Position of the end of the image frame
        /// </summary>
        public int Bottom { get;  }

        /// <summary>
        /// X Position of the end of the image frame
        /// </summary>
        public int Right { get; }
        /// <summary>
        /// Image Width in pixels
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// Imiage Height in pixels
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Vertical Resolution in pixels per inch
        /// </summary>
        public ushort VerticalResolution { get; }

        /// <summary>
        /// Horizontal Resolution in Pixels Per Inch
        /// </summary>
        public ushort HorizontalResolution { get; }

        /// <summary>
        /// Array of PaletteEntry to use for indexed images
        /// </summary>
        public PaletteEntry[] Palette { get; }

        /// <summary>
        /// A method to return Scanlines of an image
        /// </summary>
        public Func<int, byte[]> GetScanline { get; }
    }

}
