using PictSharp.ImageSharpAdaptor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using Xunit;
using static PictSharp.Tests.TestImages;

namespace PictSharp.Tests
{
    [Trait("Format", "Pict")]
    public class Encoder
    {


        public static TheoryData<Type, string, PictBpp> BitDepths = new()
        {
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit1 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit2 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit4 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit8 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit16 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit32 }
        };

        //[Theory]
        [Theory]
        [MemberData(nameof(BitDepths))]
        public void TestEncoding(Type pixelType, string filename, PictBpp bpp)
        {
           
            filename = "../../../input/" + filename;

            bool match;
            if (pixelType == typeof(Rgba32))
                match = Compare<Rgba32>(filename, bpp);
            else
                match = Compare<Rgb48>(filename, bpp);

            Assert.True(match, "Image did not match source");
            
        }

        private bool Compare<TPixel>(string filename, PictBpp bpp) where TPixel : unmanaged, IPixel<TPixel>
        {

            // Load the image
            //using var sourceImage = Image<Rgba32>.Load<Rgba32>(filename);
            var imageMagickDecoder = ImageMagickPictDecoder.Instance;

            using var sourceImage = Image<TPixel>.Load<TPixel>(filename);
            using var pictStream = new System.IO.MemoryStream();
            var encoder = new PictEncoder() { PictBpp = bpp };
                sourceImage.SaveAsPict(pictStream, encoder);

            pictStream.Flush();
            pictStream.Position = 0;

            // Reload the image in ImageMagik to to compare           
            var imageMagickImage = imageMagickDecoder.Decode<TPixel>(SixLabors.ImageSharp.Configuration.Default, pictStream);
            bool match = true;
            // very stupid compare
            for (int y = 0; y < sourceImage.Height; y++)
            {
                var sourceSpan = sourceImage.GetPixelRowSpan(y);
                var magickSpan = imageMagickImage.GetPixelRowSpan(y);
                for (int i = 0; y < sourceSpan.Length; i++)
                {
                    if (sourceSpan[y].ToScaledVector4() != magickSpan[y].ToScaledVector4())
                    {
                        match = false;
                        break;
                    }
                }
                if (match == false)
                    break;
            }

            return match;
        }
    }
}

