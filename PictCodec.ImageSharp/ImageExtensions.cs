using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace PictCodec.ImageSharpAdaptor
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts the ImageSharp Image to the PictCode ImageDetails class
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="image"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        internal static ImageDetails ToImageDetails<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
        {

            if (image == null)
                throw new ArgumentNullException(nameof(image));

            if (image.PixelType.BitsPerPixel < 32)
                throw new Exception("Index images not really supported by ImageSharp at the moment. ");

            var configuration = image.GetConfiguration();
            MemoryAllocator memoryAllocator = configuration.MemoryAllocator;

            Func<int, byte[]> getScanline = new Func<int, byte[]>(y => { 
                var pixels = image.GetPixelRowSpan(y);
                var pixelBytes = 4;
                using (IMemoryOwner<byte> row = memoryAllocator.Allocate<byte>(pixelBytes * image.Width))
                {
                    Span<byte> rowSpan = row.Memory.Span;
                    PixelOperations<TPixel>.Instance.ToBgra32Bytes(configuration, pixels, rowSpan, pixels.Length);
                    return rowSpan.ToArray();
                }

            });
        
            var imageDetails = new ImageDetails(width: image.Width,
                                                height: image.Height,
                                                bitsPerPixel: (uint)image.PixelType.BitsPerPixel,
                                                channels: 4,
                                                horizontalRes: image.Metadata.HorizontalResolution,
                                                verticalRes: image.Metadata.VerticalResolution,
                                                getScanline: getScanline);
            return imageDetails;

        }

        public static void SaveAsPict(this Image source, string path) => SaveAsPict(source, path, null);

        public static void SaveAsPict(this Image source, string path, PictEncoder encoder)
        {
            source.Save(
                path,
                encoder ?? source.GetConfiguration().ImageFormatsManager.FindEncoder(PictFormat.Instance));
        }

        public static void SaveAsPict(this Image source, Stream stream) => SaveAsPict(source, stream, null);
        public static void SaveAsPict(this Image source, Stream stream, PictEncoder encoder)
        {
            source.Save(
                stream, 
                encoder ?? source.GetConfiguration().ImageFormatsManager.FindEncoder(PictFormat.Instance));
        }
    }
}
