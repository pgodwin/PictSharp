using System;
using System.Collections.Generic;

namespace PictCodec
{
    /// <summary>
    /// Replacement for .NET "Color" class. Represents a colour in a palette, or a (non-index) pixel in an image
    /// </summary>
    public class PaletteEntry : IEqualityComparer<PaletteEntry>
    { 
        public PaletteEntry(byte a, byte r, byte g, byte b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        /// <summary>
        /// Alpha
        /// </summary>
        public byte A { get; }

        /// <summary>
        /// Red
        /// </summary>
        public byte R { get; }
        /// <summary>
        /// Green
        /// </summary>
        public byte G { get; }
        /// <summary>
        /// Blue
        /// </summary>
        public byte B { get; }


        public uint ToArgb() => BitConverter.ToUInt32(new byte[] { A, R, G, B }, 0);
        
        public uint ToRgba() => BitConverter.ToUInt32(new byte[] { R, G, B, A }, 0);

        public string ToHex() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

        public bool Equals(PaletteEntry x, PaletteEntry y)
        {
            return x.R == y.R &&
                x.G == y.G &&  
                x.B == y.B &&
                x.A == y.A;
        }

        public int GetHashCode(PaletteEntry obj)
        {
            return obj.ToHex().GetHashCode();
        }
    }

}
