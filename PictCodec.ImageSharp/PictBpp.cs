using System;
using System.Collections.Generic;
using System.Text;

namespace PictCodec.ImageSharp
{
    /// <summary>
    /// Provides enumeration for the available Pict Bit Depths.
    /// </summary>
    public enum PictBpp : byte
    {
        /// <summary>
        /// 1 bits per pixel
        /// </summary>
        Bit1 = 1,
        /// <summary>
        /// 2 bits per pixel (indexed)
        /// </summary>
        Bit2 = 2,
        /// <summary>
        /// 4 bits per pixel (indexed)
        /// </summary>
        Bit4 = 4,
        /// <summary>
        /// 8 Bits per pixel (index)
        /// </summary>
        Bit8 = 8,
        /// <summary>
        /// 16 bits per pixel (direct addressing)
        /// </summary>
        Bit16 = 16,
        /// <summary>
        /// 32 bits per pixel (direct addressing)
        /// </summary>
        Bit32 = 32,
    }
}
