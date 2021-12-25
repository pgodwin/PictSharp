using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace PictSharp.ImageSharpAdaptor
{
    public interface IPictEncodingOptions
    {
        /// <summary>
        /// Gets the quantizer for reducing the color count.
        /// </summary>
        IQuantizer Quantizer { get; }

        /// <summary>
        /// Gets the number of bits per pixel.
        /// </summary>
        PictBpp PictBpp { get; }

        bool IsIndexed { get; }



    }
}
