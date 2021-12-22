using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PictCodec.ImageSharp
{
    /**
     * This is internal to the ImageSharp library, so just reproduced here to make life easier on myself. 
     * ImageSharp Maintainers, please consider making this public for external libraries to use
     */

    /// <summary>
    /// Abstraction for shared internals for ***DecoderCore implementations to be used with <see cref="ImageEncoderUtilities"/>.
    /// </summary>
    internal interface IImageEncoderInternals
    {
        /// <summary>
        /// Encodes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <typeparam name="TPixel">The pixel type.</typeparam>
        void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>;
    }

    internal static class ImageEncoderUtilities
    {
        public static async Task EncodeAsync<TPixel>(
            this IImageEncoderInternals encoder,
            Image<TPixel> image,
            Stream stream,
            CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            Configuration configuration = image.GetConfiguration();
            if (stream.CanSeek)
            {
                await DoEncodeAsync(stream).ConfigureAwait(false);
            }
            else
            {
                using var ms = new MemoryStream();
                await DoEncodeAsync(ms);
                ms.Position = 0;
                await ms.CopyToAsync(stream, configuration.StreamProcessingBufferSize, cancellationToken)
                    .ConfigureAwait(false);
            }

            Task DoEncodeAsync(Stream innerStream)
            {
                try
                {
                    encoder.Encode(image, innerStream, cancellationToken);
                    return Task.CompletedTask;
                }
                catch (OperationCanceledException)
                {
                    return Task.FromCanceled(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }
        }

        public static void Encode<TPixel>(
            this IImageEncoderInternals encoder,
            Image<TPixel> image,
            Stream stream)
            where TPixel : unmanaged, IPixel<TPixel>
            => encoder.Encode(image, stream, default);
    }
}
