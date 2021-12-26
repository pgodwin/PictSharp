using Magick = ImageMagick;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace PictSharp.Tests
{
    internal class ImageMagickPictDecoder
    {
        public static ImageMagickPictDecoder Instance => new ImageMagickPictDecoder();

        public ImageMagickPictDecoder()
        {

        }


        private static void FromRgba32Bytes<TPixel>(Configuration configuration, Span<byte> rgbaBytes, IMemoryGroup<TPixel> destinationGroup)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Span<Rgba32> sourcePixels = MemoryMarshal.Cast<byte, Rgba32>(rgbaBytes);
            foreach (Memory<TPixel> m in destinationGroup)
            {
                Span<TPixel> destBuffer = m.Span;
                PixelOperations<TPixel>.Instance.FromRgba32(
                    configuration,
                    sourcePixels.Slice(0, destBuffer.Length),
                    destBuffer);
                sourcePixels = sourcePixels.Slice(destBuffer.Length);
            }
        }

        private static void FromRgba64Bytes<TPixel>(Configuration configuration, Span<byte> rgbaBytes, IMemoryGroup<TPixel> destinationGroup)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            foreach (Memory<TPixel> m in destinationGroup)
            {
                Span<TPixel> destBuffer = m.Span;
                PixelOperations<TPixel>.Instance.FromRgba64Bytes(
                    configuration,
                    rgbaBytes,
                    destBuffer,
                    destBuffer.Length);
                rgbaBytes = rgbaBytes.Slice(destBuffer.Length * 8);
            }
        }



        /// <summary>
        /// Returns a decoded image from ImageMagick - assumes only a singal frame, 
        /// as many methods I need are internal to ImageSharp
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Image<TPixel> Decode<TPixel>(Configuration configuration, Stream stream)
            where TPixel : unmanaged, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel>
        {
            var settings = new Magick.MagickReadSettings();

            using var magickImageCollection = new Magick.MagickImageCollection(stream, settings);

            if (magickImageCollection.Count == 0)
                throw new Exception("No images found");

            Magick.IMagickImage<ushort> magickImage = magickImageCollection[0];
            var width = magickImage.Width;
            var height = magickImage.Height;

            var image = new Image<TPixel>(configuration, width, height);


            var framePixels = image.GetPixelMemoryGroup();


            using Magick.IUnsafePixelCollection<ushort> pixels = magickImage.GetPixelsUnsafe();
            if (magickImage.Depth == 8 || magickImage.Depth == 6 || magickImage.Depth == 4 || magickImage.Depth == 2 || magickImage.Depth == 1 || magickImage.Depth == 10 || magickImage.Depth == 12)
            {
                byte[] data = pixels.ToByteArray(Magick.PixelMapping.RGBA);

                FromRgba32Bytes(configuration, data, framePixels);

            }
            else if (magickImage.Depth == 16 || magickImage.Depth == 14)
            {
                ushort[] data = pixels.ToShortArray(Magick.PixelMapping.RGBA);
                Span<byte> bytes = MemoryMarshal.Cast<ushort, byte>(data.AsSpan());
                FromRgba64Bytes(configuration, bytes, framePixels);
            }
            else
            {
                throw new InvalidOperationException();
            }
            return image;

        }
    }
}
