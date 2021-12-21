using System;
using System.Collections.Generic;
using System.Text;

namespace PictCodec.ImageSharp
{
    internal class PictConstants
    {
        public const string DefaultMimeType = "image/pict";

        public static readonly IEnumerable<string> MimeTypes = new[] { "image/pict", "image/x-pict" };

        public static readonly IEnumerable<string> FileExtensions = new[] { "pict", "pic", "pct" };
    }
}
