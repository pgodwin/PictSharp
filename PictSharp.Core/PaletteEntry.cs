using System;
using System.Collections.Generic;

namespace PictSharp
{
    /// <summary>
    /// Replacement for .NET "Color" class. Represents a colour in a palette, or a (non-index) pixel in an image
    /// </summary>
    public class PaletteEntry : IEqualityComparer<PaletteEntry>
    { 
        /// <summary>
        /// Creates a new palette entry for the colour values
        /// </summary>
        /// <param name="a">Alplha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
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

        /// <summary>
        /// Converts the colour to an Argb Value
        /// </summary>
        /// <returns></returns>
        public uint ToArgb() => BitConverter.ToUInt32(new byte[] { A, R, G, B }, 0);
        
        /// <summary>
        /// Converts the colour to an RGBA value
        /// </summary>
        /// <returns></returns>
        public uint ToRgba() => BitConverter.ToUInt32(new byte[] { R, G, B, A }, 0);

        /// <summary>
        /// Converts the colour to a HTML HEX representation
        /// </summary>
        /// <returns></returns>
        public string ToHex() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

        /// <summary>
        /// Compares two PaletteEntries to see if they are the same
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(PaletteEntry x, PaletteEntry y)
        {
            return x.R == y.R &&
                x.G == y.G &&  
                x.B == y.B &&
                x.A == y.A;
        }

        /// <summary>
        /// Calculates the hash for the palette entry
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(PaletteEntry obj)
        {
            return obj.ToHex().GetHashCode();
        }
    }

}
