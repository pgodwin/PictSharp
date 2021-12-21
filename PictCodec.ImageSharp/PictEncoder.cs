using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PictCodec.ImageSharp
{
    public class PictEncoder : IImageEncoder
    {
        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            var imagedetails = image.ToImageDetails();
            var encoder = new PictCodec.Pict();
            encoder.Encode(stream, imagedetails);
        }

        public Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
        {
            // Hacky work around for now. 
            return new Task(() => this.Encode(image, stream));
        }
    }
}
