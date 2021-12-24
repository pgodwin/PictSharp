using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec.Tests
{
    public static class TestImages
    {
        public class InputImage
        {
            public InputImage(string name,
                              int width,
                              int height)
            {
                Name = name;
                Width = width;
                Height = height;
            }

            public string Name { get; }
            public int Width { get; }
            public int Height { get; }
        }

        public static class InputImages
        {

            public static readonly InputImage Bpp1 = new InputImage("pal1.bmp", 127, 64);
            public static readonly InputImage Bpp2_Gray = new InputImage("pal2-grayscale.png", 127, 64);
            public static readonly InputImage Bpp2_Color = new InputImage("pal2-indexed.png", 127, 64);
            public static readonly InputImage Bpp4 = new InputImage("pal4.bmp", 127, 64);
            public static readonly InputImage Bpp8 = new InputImage("pal4.bmp", 127, 64);
            public static readonly InputImage Rgb555 = new InputImage("rgb16.bmp", 127, 64);
            public static readonly InputImage Rgb565 = new InputImage("rgb16-565.bmp", 127, 64);
            public static readonly InputImage Rgb24 = new InputImage("rgb24.bmp", 127, 64);
            public static readonly InputImage Rgb32 = new InputImage("rgb32.bmp", 127, 64);

            public static readonly InputImage[] All = { Bpp1, Bpp2_Gray, Bpp2_Color, Bpp4, Bpp8, Rgb555, Rgb565, Rgb24, Rgb32 };
        }

        public class InputImageProvider
        {
            public InputImageProvider(InputImage image, PixelTypes pixelType) 
            {
                Image = image;
                PixelType = pixelType;
            }

            public InputImage Image { get; }
            public PixelTypes PixelType { get; }
        }

    }
}
