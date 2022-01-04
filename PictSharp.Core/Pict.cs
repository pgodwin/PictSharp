using Be.IO;
using PictSharp.Extensions;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace PictSharp
{
    /*
     * Based on TwelveMonkeys by Harald Kuhr
     * Some ideas taken from ImageMagick's PICT coder
     * Ported to C# by Peter Godwin
     * -------------------------------
     * 
     * Copyright (c) 2008, Harald Kuhr
     * All rights reserved.
     *
     * Redistribution and use in source and binary forms, with or without
     * modification, are permitted provided that the following conditions are met:
     *     * Redistributions of source code must retain the above copyright
     *       notice, this list of conditions and the following disclaimer.
     *     * Redistributions in binary form must reproduce the above copyright
     *       notice, this list of conditions and the following disclaimer in the
     *       documentation and/or other materials provided with the distribution.
     *     * Neither the name "TwelveMonkeys" nor the
     *       names of its contributors may be used to endorse or promote products
     *       derived from this software without specific prior written permission.
     *
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
     * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
     * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
     * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
     * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
     * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
     * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
     * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
     * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
     * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
     * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
     */
    /// <summary>
    /// Implements a Pict Image Encoder
    /// </summary>
    /// <remarks>
    /// Resources: 
    ///  Inside Macintosh QuickDraw: https://web.archive.org/web/20010413132143/http://developer.apple.com/techpubs/mac/QuickDraw/QuickDraw-2.html
    ///  QuickDraw TechNotes: https://web.archive.org/web/20010414045329/http://developer.apple.com/technotes/indexes/qd-a.html
    /// </remarks>
    public class Pict : PictBase, IEncoder
    {


        /// <inheritdoc/>
        public void Encode(Stream output, ImageDetails image) => this.Encode(output, image, CancellationToken.None);
        
        /// <inheritdoc/>
        public void Encode(Stream output, ImageDetails image, CancellationToken cancellationToken)
        {
            using (var stream = new BeBinaryWriter(output, Encoding.Default, true))
            {
                var h = image.Height;
                int size = 0;

                this.WriteHeader(stream, image);

                for (int y = 0; y < h; y++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        stream.Flush();
                        break;
                    }

                    var row = image.GetScanline(y);
                    size += this.WritePixelScanLine(stream, image, row);
                }

                this.WriteTrailer(stream, size);

            }

        }

        private int rowBytes;
        private int packedBytes;
        private int sourceRowBytes;

        private ushort ScaleQuantumToShort(byte quantum)
        {
            if (quantum <= 0.0)
                return (0);
            if ((257.0 * quantum) >= 65535.0)
                return (65535);
            return ((ushort)(257.0 * quantum + 0.5));

        }



        internal void WriteHeader(BeBinaryWriter imageOutput, ImageDetails imageDetails)
        {
            // If the image is indexed, it'll always be a single channel, otherwise PICT is always 3 Channels + Alpha
            var pictComponents = (imageDetails.IsIndexed ? 1 : 4);
            // If a source image is 24 bit, return 32 bit
            // Otherwise 1, 4, 8, 16 and 32 bit images supported.
            var pictBps = imageDetails.BitsPerPixel == 24 ? 32 : imageDetails.BitsPerPixel;
            // I've seen all sorts of weird things
            PackType packType = PackType.PackBits;




            if (imageDetails.BitsPerPixel == 1)
            {
                packedBytes = (imageDetails.Width / 8 + ((imageDetails.Width % 8) != 0 ? 1 : 0));
                rowBytes = packedBytes;
                sourceRowBytes = rowBytes;
                packType = PackType.PackBits;
            }
            if (imageDetails.BitsPerPixel == 2)
            {
                packedBytes = (imageDetails.Width / 4 + ((imageDetails.Width % 4) != 0 ? 1 : 0));
                rowBytes = packedBytes;
                sourceRowBytes = rowBytes;
                packType = PackType.PackBits;
            }
            if (imageDetails.BitsPerPixel == 4)
            {
                packedBytes = (imageDetails.Width / 2 + ((imageDetails.Width % 2) != 0 ? 1 : 0));
                rowBytes = packedBytes;
                sourceRowBytes = rowBytes;
                packType = PackType.PackBits;
            }
            else if (imageDetails.BitsPerPixel == 8)
            {
                packedBytes = (int)(((imageDetails.Width * 8) / imageDetails.BitsPerPixel) +
                                (((imageDetails.Width * 8) % imageDetails.Width) != 0 ? 1 : 0));
                rowBytes = imageDetails.Width * 1;
                sourceRowBytes = rowBytes;
                packType = PackType.PackBits;
            }
            else if (imageDetails.BitsPerPixel == 16)
            {
                packedBytes = imageDetails.Width * 2;
                rowBytes = imageDetails.Width * 2;
                sourceRowBytes = imageDetails.Width * 2;
                packType = PackType.PackBits;
            }
            else if (imageDetails.BitsPerPixel == 24)
            {
                packedBytes = imageDetails.Width * 4;
                rowBytes = imageDetails.Width * 4;
                sourceRowBytes = imageDetails.Width * 3;
                packType = PackType.PackBitsRgb;
            }
            else if (imageDetails.BitsPerPixel == 32)
            {
                packedBytes = imageDetails.Width * pictComponents;
                rowBytes = imageDetails.Width * pictComponents;
                sourceRowBytes = imageDetails.Width * 4;
                packType = PackType.PackBitsRgb;
            }

            if (rowBytes < 8)
                packType = PackType.NotPacked;


            // TODO: Make 512 byte header optional
            // Write empty 512-byte header
            byte[] buf = new byte[Pict.PICT_NULL_HEADER_SIZE];
            imageOutput.Write(buf);

            // Write out the size, leave as 0, this is ok
            imageOutput.WriteShort(0);

            // Write image frame (same as image bounds)
            imageOutput.WriteShort(imageDetails.Top);
            imageOutput.WriteShort(imageDetails.Left);
            imageOutput.WriteShort(imageDetails.Bottom);
            imageOutput.WriteShort(imageDetails.Right);

            // Write version, version 2
            imageOutput.WriteShort(Pict.OP_VERSION);
            imageOutput.WriteShort(Pict.OP_VERSION_2);

            // Version 2 HEADER_OP, extended version.
            imageOutput.WriteShort(Pict.OP_HEADER_OP);
            imageOutput.WriteInt(Pict.HEADER_V2_EXT); // incl 2 bytes reseverd
            //imageOutput.WriteShort(FFEE); // FFEF or FFEE
            //imageOutput.WriteShort(0x0); // Reservered 

            // Original Horizontal Resolution in Pixels / Inch
            // Image resolution, 72 dpi (long)
            imageOutput.WriteShort(72);
            imageOutput.WriteShort(0);
            // Original Verticale Resolution in Pixels / Inch
            imageOutput.WriteShort(72);
            imageOutput.WriteShort(0);

            // Optimal source rectangle (same as image bounds)
            // Frame at original resolution
            imageOutput.WriteShort(imageDetails.Top);
            imageOutput.WriteShort(imageDetails.Left);
            imageOutput.WriteShort(imageDetails.Bottom);
            imageOutput.WriteShort(imageDetails.Right);

            // Reserved (4 bytes)
            imageOutput.WriteInt(0);

            // ------------------ END OF HEADER -------------------- //

            // This is where things get weird...depending on bpp






            // Set the clip rectangle
            imageOutput.WriteShort(Pict.OP_CLIP_RGN);
            imageOutput.WriteShort(10);
            imageOutput.WriteShort(imageDetails.Top); // top
            imageOutput.WriteShort(imageDetails.Left); // left
            imageOutput.WriteShort(imageDetails.Bottom);
            imageOutput.WriteShort(imageDetails.Right);

            if (imageDetails.IsIndexed)
            {
                if (imageDetails.BitsPerPixel == 1)
                {
                    imageOutput.WriteShort(Pict.OP_BITS_RECT);
                    imageOutput.WriteShort(rowBytes);

                    // Write bounds rectangle (same as image bounds)
                    imageOutput.WriteShort(imageDetails.Top); // top
                    imageOutput.WriteShort(imageDetails.Left); // left
                    imageOutput.WriteShort(imageDetails.Bottom); // TODO: Handle overflow? // bottom
                    imageOutput.WriteShort(imageDetails.Right); // right

                    // Source and dest rect (both are same as image bounds)
                    imageOutput.WriteShort(imageDetails.Top);
                    imageOutput.WriteShort(imageDetails.Left);
                    imageOutput.WriteShort(imageDetails.Bottom);
                    imageOutput.WriteShort(imageDetails.Right);

                    // Dest Rect
                    imageOutput.WriteShort(imageDetails.Top);
                    imageOutput.WriteShort(imageDetails.Left);
                    imageOutput.WriteShort(imageDetails.Bottom);
                    imageOutput.WriteShort(imageDetails.Right);

                    // Transfer mode
                    imageOutput.WriteShort(0);
                    imageOutput.Flush();

                    return;

                }
                else
                {
                    imageOutput.WriteShort(Pict.OP_PACK_BITS_RECT); // Packbits
                    // The offset in bytes from one row of the image to the next. The value must be even, less than $4000, and for best performance it should be a multiple of 4. 
                    // The high 2 bits of rowBytes are used as flags. If bit 15 = 1, the data structure pointed to is a PixMap record; otherwise it is a BitMap record.
                    imageOutput.WriteUShort((ushort)(rowBytes | 0x8000));
                }
            }
            else // RGB Image
            {
                // Pixmap operation
                imageOutput.WriteShort(Pict.OP_DIRECT_BITS_RECT); // 0x9A - sometimes called pict9a
                // PixMap pointer (always 0x000000FF);
                imageOutput.WriteInt(0xff);
                // I see conflicting things about writing out row bytes at this point...
                imageOutput.WriteUShort((ushort)(rowBytes | 0x8000));
            }


            // Write bounds rectangle (same as image bounds)
            imageOutput.WriteShort(imageDetails.Top); // top
            imageOutput.WriteShort(imageDetails.Left); // left
            imageOutput.WriteShort(imageDetails.Bottom); // TODO: Handle overflow? // bottom
            imageOutput.WriteShort(imageDetails.Right); // right



            // PixMap record version
            // The version number of Color QuickDraw that created this PixMap record. 
            // The value of pmVersion is normally 0. If pmVersion is 4, Color QuickDraw treats the PixMap record's baseAddr field as 32-bit clean.
            // (All other flags are private.) Most applications never need to set this field.
            imageOutput.WriteShort(0);

            // Packing format (always 4: PackBits)
            // * 0 is default indexed packing.
            // * 1 is no packing (rowBytes < 8)
            // * 2 
            // * 3
            // * 4 is default direct packing - run length encoded scan lines by component, red first.
            imageOutput.WriteShort(packType);

            // Size of packed data (leave as 0)
            // The size of the packed image in bytes. When the packType field contains the value 0, this field is always set to 0
            imageOutput.WriteInt(0);

            // Pixmap resolution, 72 dpi
            //imageOutput.WriteShort(Pict.MAC_DEFAULT_DPI+0.5);
            imageOutput.WriteShort(72);
            imageOutput.WriteShort(0);
            //imageOutput.WriteShort(Pict.MAC_DEFAULT_DPI+0.5);
            imageOutput.WriteShort(72);
            imageOutput.WriteShort(0);

            // Pixel type
            // The storage format for a pixel image. Indexed pixels are indicated by a value of 0. 
            // Direct pixels are specified by a value of RGBDirect, or 16.
            // In the PixMap record of the GDevice record (described in the chapter "Graphics Devices") 
            // for a direct device, this field is set to the constant RGBDirect when the screen depth is set.
            if (imageDetails.IsIndexed)
                imageOutput.WriteShort(0);
            else
                imageOutput.WriteShort(16);


            // Pixel size
            // Pixel depth; that is, the number of bits used to represent a pixel. 
            // Indexed pixels can have sizes of 1, 2, 4, and 8 bits; direct pixel sizes are 16 and 32 bits.
            imageOutput.WriteShort(pictBps);

            // Pixel component count (planes) - ie 1 for indexed, 3 or 4 for RGB
            imageOutput.WriteShort(pictComponents);

            // Pixel component size
            /* 
               The size in bits of each component for a pixel. Color QuickDraw expects that the sizes of all components 
               are the same, and that the value of the cmpCount field multiplied by the value of the cmpSize field 
               is less than or equal to the value in the pixelSize field.
               For an indexed pixel value, which has only one component, the value of the cmpSize field is the same
               as the value of the pixelSize field--that is, 1, 2, 4, or 8.
               For direct pixels there are two additional possibilities:
               A 16-bit pixel, which has three components, has a cmpSize value 
               of 5. This leaves an unused high-order bit, which Color QuickDraw sets to 0.
               A 32-bit pixel, which has three components (red, green, and blue), has a cmpSize value of 8.
               This leaves an unused high-order byte, which Color QuickDraw sets to 0.
            */
            if (imageDetails.IsIndexed)
                imageOutput.WriteShort(pictBps);
            else if (imageDetails.BitsPerPixel == 16)
                imageOutput.WriteShort(5);
            else
                imageOutput.WriteShort(8);

            // PlaneBytes, ignored for now
            imageOutput.WriteInt(0);

            // TODO: Allow IndexColorModel?
            // ColorTable record (for RGB direct pixels, just write 0)
            // Pixmap Colour Table (seems to always be 0?)
            imageOutput.WriteInt(0);
            //imageOutput.WriteInt(0x101F);

            // Reserved (4 bytes)
            imageOutput.WriteInt(0);

            // Write out ColorTable
            if (imageDetails.IsIndexed)
            {
                imageOutput.WriteInt(0); // color seed - Resource ID
                // Colour Flags Flags. A value of $0000 identifies this as a color table for a pixel map. A value of $8000 identifies this as a color table for an indexed device.
                imageOutput.WriteShort(0);
                // Entry count / Size. One less than the number of color specification entries in the rest of this resource.
                imageOutput.WriteShort((ushort)(imageDetails.Palette.Length - 1));
                for (ushort i = 0; i < imageDetails.Palette.Length; i++)
                {
                    imageOutput.WriteShort(i); // pixel value

                    // Each colour is a 16bit value...scale to short
                    imageOutput.WriteUShort(ScaleQuantumToShort(imageDetails.Palette[i].R));
                    imageOutput.WriteUShort(ScaleQuantumToShort(imageDetails.Palette[i].G));
                    imageOutput.WriteUShort(ScaleQuantumToShort(imageDetails.Palette[i].B));
                }
            }

            // Source and dest rect (both are same as image bounds)
            imageOutput.WriteShort(imageDetails.Top);
            imageOutput.WriteShort(imageDetails.Left);
            imageOutput.WriteShort(imageDetails.Bottom);
            imageOutput.WriteShort(imageDetails.Right);

            // Dest Rect
            imageOutput.WriteShort(imageDetails.Top);
            imageOutput.WriteShort(imageDetails.Left);
            imageOutput.WriteShort(imageDetails.Bottom);
            imageOutput.WriteShort(imageDetails.Right);

            // Transfer mode
            if (imageDetails.IsIndexed)
                imageOutput.WriteShort(0);
            else
                imageOutput.WriteShort(/*QuickDraw.SRC_COPY*/0x40);

            imageOutput.Flush();

            // Now write image data

        }

        /// <summary>
        /// Writes and compresses (if needed) each scan line
        /// </summary>
        /// <param name="imageOutput"></param>
        /// <param name="imageDetails"></param>
        /// <param name="scanLine"></param>
        /// <returns></returns>
        internal int WritePixelScanLine(BeBinaryWriter imageOutput, ImageDetails imageDetails, byte[] scanLine)
        {
            int xOffset = 0;
            var pixels = scanLine;
            int w = imageDetails.IsIndexed ? packedBytes : imageDetails.Width;

            uint bytesPerPixel = imageDetails.IsIndexed ? 1 : imageDetails.BitsPerPixel / 8;
            uint pixelsPerByte = 8 / imageDetails.BitsPerPixel;

            byte[] scanlineBytes = new byte[rowBytes];
            bool invert = imageDetails.BitsPerPixel == 1;

            // Mask for Pixel Packing based on bits per pixel
            int packMask = (1 << (int)imageDetails.BitsPerPixel) - 1;

            // Masks for RGB555 (16bit) encoding
            int redMask = 0x7C00;
            int greenMask = 0x3E0;
            int blueMask = 0x1F;



            int byteCount = 0;



            // Treat the scanline.
            for (int x = 0; x < w; x++)
            {
                var colorIndex = x * bytesPerPixel;
                if (imageDetails.IsIndexed)
                {
                    // 8 Bit images
                    if (pixelsPerByte == 1)
                    {
                        scanlineBytes[x] = pixels[x];
                    }
                    else
                    {

                        // Pack 1, 2, 4 bit image palette entries into a single byte
                        int shift = 8 - (int)imageDetails.BitsPerPixel;
                        byte packedPixel = 0;
                        xOffset = x * (int)pixelsPerByte;

                        // Calculate the reamining pixels from the end of the buffer
                        int remaining = 0;
                        byte pixel;

                        
                        if (xOffset >= pixels.Length - pixelsPerByte)
                            remaining = (int)pixelsPerByte - (pixels.Length % (int)pixelsPerByte); ;
                        for (int j = 0; j < pixelsPerByte - remaining; j++)
                        {
                           
                           pixel = pixels[xOffset + j];

                            // 1bpp images are inverted, that is zero is white.
                            if (invert)
                                pixel = (byte)~pixel; // reverse the pixel value

                            packedPixel = (byte)(packedPixel | ((byte)(pixel & packMask) << shift));
                            shift -= (int)imageDetails.BitsPerPixel;
                        }
                        scanlineBytes[x] = packedPixel;
                        
                        
                    }
                }
                else if (imageDetails.BitsPerPixel == 16)
                {
                    // not working yet, not sure if it's the header or what...
                    var pixelShort = (short)((pixels[colorIndex + 0] << 8) + pixels[colorIndex + 1]);
                    byte r = (byte)(((pixelShort & redMask) >> 10) << 3);
                    byte g = (byte)(((pixelShort & greenMask) >> 5) << 3);
                    byte b = (byte)((pixelShort & blueMask) << 3);
                    
                    var color = new PaletteEntry(255, r, g, b);

                    // this is the colour at this location, but we can probably just copy it back out?
                    scanlineBytes[colorIndex] = pixels[colorIndex +1];
                    scanlineBytes[colorIndex+1] = pixels[colorIndex];


                }
                else // True color 
                {
                    PaletteEntry color;
                    if (imageDetails.BitsPerPixel == 32)
                        color = new PaletteEntry(pixels[colorIndex + 3], pixels[colorIndex + 2], pixels[colorIndex + 1], pixels[colorIndex + 0]);
                    else //(imageDetails.BitsPerPixel == 24)
                        color = new PaletteEntry(255, pixels[colorIndex + 2], pixels[colorIndex + 1], pixels[colorIndex + 0]);
                 
                    // Write out as rows of pixels, 1 per channel
                    scanlineBytes[xOffset + x]          = color.A;
                    scanlineBytes[xOffset + w + x]      = color.R;
                    scanlineBytes[xOffset + 2 * w + x]  = color.G;
                    scanlineBytes[xOffset + 3 * w + x]  = color.B; 
                }
            }

            // Pack using PackBitsEncoder
            // https://web.archive.org/web/20080705155158/http://developer.apple.com/technotes/tn/tn1023.html
            // Small images aren't compressed
            if (rowBytes < 8)
            {
                byteCount += scanlineBytes.Length;
                imageOutput.BaseStream.Write(scanlineBytes, 0, scanlineBytes.Length);
            }
            // TODO Check the compression setting
            else // Pack the bytes
            {
                var packedBytes = PackBitsCompressor.PackBits(scanlineBytes);
                //var packedBytes = new PackBitsEncoder().compress(scanlineBytes);
                if (rowBytes > 250) // apple says 250, ImageMagick has 200...hmm
                {
                    imageOutput.WriteShort((short)packedBytes.Length);
                    byteCount += 2;
                }
                else
                {
                    imageOutput.Write((byte)packedBytes.Length);
                    byteCount++;
                }

                byteCount += packedBytes.Length;
                imageOutput.BaseStream.Write(packedBytes, 0, packedBytes.Length);
            }

            imageOutput.Flush();

            
            return byteCount;

        }



        internal void WriteTrailer(BeBinaryWriter imageOutput, int length)
        {

            // Write out end opcode. Be sure to be word-aligned.
            if (length == -1)
            {
                throw new IOException("Cannot write trailer without knowing length");
            }

            // Make sure we have an even number of bytes
            if ((length & 1) != 0)
            {
                imageOutput.Write((byte)0);
            }

            imageOutput.WriteShort(Pict.OP_END_OF_PICTURE);
            imageOutput.Flush();

            var offset = imageOutput.Seek(512, SeekOrigin.Begin);
            imageOutput.WriteUShort((ushort)offset);

            imageOutput.Flush();

        }


        /// <summary>
        /// Performs final shifting from a 5bit value to an 8bit one.
        /// </summary>
        /// <param name="value">The masked and shifted value.</param>
        /// <returns>The <see cref="byte"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte GetBytesFrom5BitValue(int value) => (byte)((value << 3) | (value >> 2));

        /// <summary>
        /// Performs final shifting from a 6bit value to an 8bit one.
        /// </summary>
        /// <param name="value">The masked and shifted value.</param>
        /// <returns>The <see cref="byte"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte GetBytesFrom6BitValue(int value) => (byte)((value << 2) | (value >> 4));



    }
}
