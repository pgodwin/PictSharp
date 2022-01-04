using System;
using System.Collections.Generic;
using System.Text;

namespace PictSharp
{
    /// <summary>
    /// Color QuickDraw has always supported packed PICTs. See Inside Macintosh, Volume V, for details on how CLUT PixMaps are packed. 
    /// Under 32-Bit QuickDraw, to pack direct RGB PixMaps in PICTs, call CopyBits with the packType field in the source PixMap
    /// set to one of the following constants that apply to direct RGB PixMaps.
    /// 
    /// pixelSize 16 defaults to packType 3 and pixelSize 32 defaults to packType 4
    /// </summary>
    public enum PackType : short
    {
        /// <summary>
        /// This packType indicates that the data was packed by run-length encoding
        /// bytes of data with PackBits.
        /// </summary>
        PackBits = 0,
        /// <summary>
        /// This PackType indicates that the data was not packed at all (rowBytes less than 8)
        /// </summary>
        NotPacked = 1,

        /// <summary>
        /// This PackType indicates that the data was packed by removing the alpha
        /// channel from each pixel. 
        /// </summary>
        StripAlpha = 2,
        /// <summary>
        /// If the packType field holds the value 3 and the pixel map is 16 bits per pixel, 
        /// then run-length encoding is done, but not through PackBits. 
        /// Instead, a run-length encoding algorithm private to QuickDraw is used. 
        /// This algorithm is very similar to PackBits, but where PackBits compresses runs of bytes, 
        /// this routine compresses runs of words. 
        /// </summary>
        PackBitsPixel = 3,
        /// <summary>
        /// If the packType field holds the value 4 and the pixel map is 32-bits per pixel, 
        /// then run-length encoding via PackBits is done, but only after some preprocessing is done. 
        /// QuickDraw first rearranges the color components of the pixels so that each color component 
        /// of every pixel is consecutive.
        /// </summary>
        /// <remarks>
        /// Scheme 4 will store the alpha channel also if cmpCount is set to to four. 
        /// PackSize is not used and should be set to zero for compatibility reasons. 
        /// These are the only compression techniques supported at this time.
        /// </remarks>
        PackBitsRgb = 4
    }
}
