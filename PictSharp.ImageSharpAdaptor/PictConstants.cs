﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PictSharp.ImageSharpAdaptor
{
    internal class PictConstants
    {
        public const string DefaultMimeType = "image/pict";

        public static readonly IEnumerable<string> MimeTypes = new[] { "image/pict", "image/x-pict" };

        public static readonly IEnumerable<string> FileExtensions = new[] { "pict", "pic", "pct" };
    }
}
