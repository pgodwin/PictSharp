using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PictSharp.ImageSharpAdaptor
{
    public class PictEncoder : IImageEncoder, IPictEncodingOptions
    {
        /// <inheritdoc/>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public IQuantizer Quantizer { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <inheritdoc/>
        public PictBpp PictBpp { get; set; }

        public bool IsIndexed { get; set; }

        /// <inheritdoc/>
        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            using var encoder = new PictEncoderCore(image.GetConfiguration().MemoryAllocator, image.GetConfiguration(), new PictEncodingOptions(this));
            encoder.Encode(image, stream);
        }

        /// <inheritdoc/>
        public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
        {
            // The introduction of a local variable that refers to an object the implements
            // IDisposable means you must use async/await, where the compiler generates the
            // state machine and a continuation.
            using var encoder = new PictEncoderCore(image.GetConfiguration().MemoryAllocator, image.GetConfiguration(), new PictEncodingOptions(this));
            await encoder.EncodeAsync(image, stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
