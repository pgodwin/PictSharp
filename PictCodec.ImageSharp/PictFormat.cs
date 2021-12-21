using SixLabors.ImageSharp.Formats;
using System.Collections.Generic;

namespace PictCodec.ImageSharp
{
    /// <summary>
    /// Registers the image encoder, decoders and mime type detectors for the Apple PICT format.
    /// </summary>
    public class PictFormat : IImageFormat<PictMetadata>
    {
        public string Name => "PICT";

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static PictFormat Instance { get; } = new PictFormat();

        public string DefaultMimeType => PictConstants.DefaultMimeType;

        public IEnumerable<string> MimeTypes => PictConstants.MimeTypes;

        public IEnumerable<string> FileExtensions => PictConstants.FileExtensions;

        public PictMetadata CreateDefaultFormatMetadata() => new PictMetadata();

    }
}
