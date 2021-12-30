using PictSharp.ImageSharpAdaptor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static PictSharp.Tests.TestImages;

namespace PictSharp.Tests
{
    [Trait("Format", "Pict")]
    public class Encoder
    {


        public static TheoryData<Type, string, PictBpp> BitDepths = new TheoryData<Type, string, PictBpp>
        {
            // Skip 16-bit and 1bpp images
            // 16bpp encoding is broken
            // 1bpp decoding in ImageMagik is broken
            //{ typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit1 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit2 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit4 },
            { typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit8 },
            //{ typeof(Rgba32), TestImages.InputImages.Rgb32.Name, PictBpp.Bit16 },
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
            var outputPath = "../../../output/" + Path.GetFileNameWithoutExtension(filename);
            var pictPath = $"{outputPath}_{bpp}.pict";
            using var sourceImage = Image<TPixel>.Load<TPixel>(filename);
            using var pictStream = new System.IO.MemoryStream();

            var maxColors = ((byte)bpp >= 16 ? 256 : 1 << (int)bpp);
            // Only use two colors, as PNG support palletised 1 and 2 bit images, whereas PICT doesn't
            //var colors = new ReadOnlyMemory<Color>(new Color[] { Color.Black, Color.White });
            //var quantizer = new PaletteQuantizer(colors, new QuantizerOptions { MaxColors = maxColors });
            var quantizer = new WuQuantizer(new QuantizerOptions { MaxColors = maxColors }); 

            var encoder = new PictEncoder() { PictBpp = bpp, Quantizer = quantizer };
            sourceImage.SaveAsPict(pictPath, encoder);
            PngBitDepth pngBpp;
            PngColorType pngColorType;

            switch (bpp)
            {
                case PictBpp.Bit1:
                    pngBpp = PngBitDepth.Bit1;
                    pngColorType = PngColorType.Palette;
                    break;
                case PictBpp.Bit2:
                    pngBpp = PngBitDepth.Bit2;
                    pngColorType = PngColorType.Palette;
                    break;
                case PictBpp.Bit4:
                    pngBpp = PngBitDepth.Bit4;
                    pngColorType = PngColorType.Palette;
                    break;
                case PictBpp.Bit8:
                    pngBpp = PngBitDepth.Bit8;
                    pngColorType = PngColorType.Palette;
                    break;
                default:
                    pngBpp = PngBitDepth.Bit8;
                    pngColorType = PngColorType.Rgb;
                    break;
            }
            // For reference, also write out a PNG version
            sourceImage.SaveAsPng($"{outputPath}_{bpp}.png", new SixLabors.ImageSharp.Formats.Png.PngEncoder() { BitDepth = pngBpp, ColorType = pngColorType, Quantizer = quantizer });

            // Get the quantized version to compare
            using var pngVersion = Image.Load<TPixel>($"{outputPath}_{bpp}.png");
            // Reload the image in ImageMagik to to compare           
            using var imageMagickImage = imageMagickDecoder.ReadPict<TPixel>(SixLabors.ImageSharp.Configuration.Default, pictPath);
            bool match = true;
            // very stupid compare
            for (int y = 0; y < pngVersion.Height; y++)
            {
                var sourceSpan = pngVersion.GetPixelRowSpan(y);
                var magickSpan = imageMagickImage.GetPixelRowSpan(y);
                for (int x = 0; x < sourceSpan.Length; x++)
                {
                    if (sourceSpan[x].ToScaledVector4() != magickSpan[x].ToScaledVector4())
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

