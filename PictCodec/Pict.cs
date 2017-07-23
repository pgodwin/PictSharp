using Be.IO;
using PictCodec;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
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
    public class Pict : PictBase, IEncoder
    {
        private int rowBytes;

        private ushort ScaleQuantumToShort(byte quantum)
        {
            if (quantum <= 0.0)
                return (0);
            if ((257.0 * quantum) >= 65535.0)
                return (65535);
            return ((ushort) (257.0 * quantum + 0.5));

        }

        public void Encode(Stream output, Bitmap bitmap)
        {
            var targetBmp = bitmap;
            //using (Bitmap targetBmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format8bppIndexed))
            {
                // targetBmp is now in the desired format.

                
                using (var stream = new BeBinaryWriter(output, Encoding.Default, true))
                {
                    writePICTHeader(stream, targetBmp);

                    //writePICTData(0, 0, bitmap.Width, bitmap.Height, new ColorModel(), )

                    var size = writePICTData(stream, targetBmp);

                    writePICTTrailer(stream, size);

                    stream.Flush();
                }
            }
        }

        private void writePICTHeader(BeBinaryWriter imageOutput, Bitmap pImage)
        {
            var imageDetails = pImage.GetImageDetails();

            
            // TODO: Make 512 byte header optional
            // Write empty 512-byte header
            byte[] buf = new byte[Pict.PICT_NULL_HEADER_SIZE];
            imageOutput.Write(buf);

            // Write out the size, leave as 0, this is ok
            imageOutput.WriteShort(0);

            // Write image frame (same as image bounds)
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(pImage.Height);
            imageOutput.WriteShort(pImage.Width);

            // Write version, version 2
            imageOutput.WriteShort(Pict.OP_VERSION);
            imageOutput.WriteShort(Pict.OP_VERSION_2);

            // Version 2 HEADER_OP, extended version.
            imageOutput.WriteShort(Pict.OP_HEADER_OP);
            imageOutput.WriteInt(Pict.HEADER_V2_EXT); // incl 2 bytes reseverd

            // Image resolution, 72 dpi
            imageOutput.WriteShort((ushort)Math.Ceiling(pImage.VerticalResolution));
            imageOutput.WriteShort(0);
            imageOutput.WriteShort((ushort)Math.Ceiling(pImage.HorizontalResolution));
            imageOutput.WriteShort(0);

            // Optimal source rectangle (same as image bounds)
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(pImage.Height);
            imageOutput.WriteShort(pImage.Width);

            // Reserved (4 bytes)
            imageOutput.WriteInt(0);


            // TODO: The header really ends here...

            // Highlight
            //imageOutput.WriteShort(Pict.OP_DEF_HILITE);

            // Set the clip rectangle
            imageOutput.WriteShort(Pict.OP_CLIP_RGN);
            imageOutput.WriteShort(10);
            imageOutput.WriteUShort(0); // top
            imageOutput.WriteUShort(0); // left
            imageOutput.WriteUShort(pImage.Height); // bottom
            imageOutput.WriteUShort(pImage.Width); // right

            if (imageDetails.IsIndexed)
            {
                imageOutput.WriteShort(0x98); // PictPICTOp
                
            }
            else
            {
                // Pixmap operation
                imageOutput.WriteShort(Pict.OP_DIRECT_BITS_RECT); // 0x9A

                // PixMap pointer (always 0x000000FF);
                imageOutput.WriteInt(0xff);
            }

            // Write rowBytes, this is 4 times the width.
            // Set the high bit, to indicate a PixMap.

            //rowBytes = 4 * pImage.Width;
            rowBytes = imageDetails.Channels * pImage.Width;

            // The offset in bytes from one row of the image to the next. The value must be even, less than $4000, and for best performance it should be a multiple of 4. 
            // The high 2 bits of rowBytes are used as flags. If bit 15 = 1, the data structure pointed to is a PixMap record; otherwise it is a BitMap record.
            //imageOutput.WriteUShort((ushort)(rowBytes | unchecked((short)0x8000)));
            imageOutput.WriteUShort((ushort)(rowBytes | 0x8000));

            // Write bounds rectangle (same as image bounds)
            imageOutput.WriteShort(0); // top
            imageOutput.WriteShort(0); // left
            imageOutput.WriteShort(pImage.Height); // TODO: Handle overflow? // bottom
            imageOutput.WriteShort(pImage.Width); // right
            
            // PixMap record version
            // The version number of Color QuickDraw that created this PixMap record. 
            // The value of pmVersion is normally 0. If pmVersion is 4, Color QuickDraw treats the PixMap record's baseAddr field as 32-bit clean.
            // (All other flags are private.) Most applications never need to set this field.
            imageOutput.WriteShort(0);

            // Packing format (always 4: PackBits)
            // * 0 is default indexed packing.
            // * 1 is no packing(rowBytes < 8)
            // * 4 is default direct packing - run length encoded scan lines by component, red first.
            if (rowBytes < 8)
                imageOutput.WriteShort(1);
            else if (imageDetails.IsIndexed)
                imageOutput.WriteShort(0);
            else
                imageOutput.WriteShort(4);

            // Size of packed data (leave as 0)
            // The size of the packed image in bytes. When the packType field contains the value 0, this field is always set to 0
            imageOutput.WriteInt(0);

            // Pixmap resolution, 72 dpi
            //imageOutput.WriteShort(Pict.MAC_DEFAULT_DPI+0.5);
            imageOutput.WriteShort((ushort)Math.Ceiling(pImage.VerticalResolution+0.5));
            imageOutput.WriteShort(0);
            //imageOutput.WriteShort(Pict.MAC_DEFAULT_DPI+0.5);
            imageOutput.WriteShort((ushort)Math.Ceiling(pImage.HorizontalResolution+0.5));
            //imageOutput.WriteUShort(38);
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
            imageOutput.WriteShort(imageDetails.BitsPerPixel);

            // Pixel component count
            imageOutput.WriteShort(imageDetails.Channels);

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
                imageOutput.WriteShort(imageDetails.BitsPerPixel);
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

            // Indexed Colour:
            if (imageDetails.IsIndexed)
            {
                imageOutput.WriteInt(0); // color seed
                imageOutput.WriteShort(0); // Colour Flags
                imageOutput.WriteShort((ushort) (pImage.Palette.Entries.Length - 1)); // Entry count 
                for (ushort i = 0; i < pImage.Palette.Entries.Length; i++)
                {
                    imageOutput.WriteUShort(i); // pixel value
                    // Each colour is a 16bit value...scale to short
                    imageOutput.WriteUShort(ScaleQuantumToShort(pImage.Palette.Entries[i].R));
                    imageOutput.WriteUShort(ScaleQuantumToShort(pImage.Palette.Entries[i].G));
                    imageOutput.WriteUShort(ScaleQuantumToShort(pImage.Palette.Entries[i].B));
                    
                }
            }

            // Source and dest rect (both are same as image bounds)
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(pImage.Height);
            imageOutput.WriteShort(pImage.Width);
            // Dest Rect
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(0);
            imageOutput.WriteShort(pImage.Height);
            imageOutput.WriteShort(pImage.Width);

            // Transfer mode
            if (imageDetails.IsIndexed)
                imageOutput.WriteShort(0);
            else
                imageOutput.WriteShort(/*QuickDraw.SRC_COPY*/0x40);

            imageOutput.Flush();

            // Now write image data

        }

        private int writePICTData(BeBinaryWriter imageOutput, Bitmap bitmap)
        {

            var imageDetails = bitmap.GetImageDetails();

            var components = imageDetails.Channels;

            int scanWidthLeft = bitmap.Width;

            int xOffset = 0;
            int yOffset = 0;
            int w = bitmap.Width;
            int h = bitmap.Height;

            var pixels = bitmap.ToByteArray();
            var bmpStride = pixels.Length / h;
            int scansize = bitmap.Width;

            byte[] scanlineBytes = new byte[rowBytes];

            int byteCount = 0;

            // Fill the scanline buffer. 
            for (int y = 0; y < h; y++)
            {
                // Reduce the counter of bytes left on the scanline.
                scanWidthLeft -= w;
                // Treat the scanline.
                for (int x = 0; x < w; x++)
                {
                    var colorIndex = y * bmpStride + x * components;

                    if (imageDetails.IsIndexed)
                    {
                        scanlineBytes[x] =  (byte)pixels[colorIndex];
                    }
                    else // True color 
                    {
                        Color color;
                        if (components == 4 && bitmap.PixelFormat == PixelFormat.Format32bppArgb)
                            color = Color.FromArgb(pixels[colorIndex + 3], pixels[colorIndex + 2], pixels[colorIndex + 1], pixels[colorIndex + 0]);
                        else
                            color = Color.FromArgb(255, pixels[colorIndex + 2], pixels[colorIndex + 1], pixels[colorIndex + 0]);
                        
                        // Todo FIX 16 and 24-bit images

                        scanlineBytes[xOffset + x] = color.A;
                        scanlineBytes[xOffset + w + x] = color.R;
                        scanlineBytes[xOffset + 2 * w + x] = color.G ;
                        scanlineBytes[xOffset + 3 * w + x] = color.B;
                    }
                }

                // https://web.archive.org/web/20080705155158/http://developer.apple.com/technotes/tn/tn1023.html

                // If we have a complete scanline, then pack it and write it out.
                if (scanWidthLeft == 0)
                {
                    // thinking the encoder needs to know something about our row bytes
                    // Pack using PackBitsEncoder

                    // Small images aren't compressed
                    if (rowBytes < 8)
                    {
                        byteCount += scanlineBytes.Length;
                        imageOutput.BaseStream.Write(scanlineBytes, 0, scanlineBytes.Length);
                    }
                    else // Pack the bytes
                    {
                        var encoder = new PackBitsEncoder();
                        
                        var packedBytes = encoder.compress(scanlineBytes);

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
                    
                    scanWidthLeft = w;
                    
                }
            }
            return byteCount;

        }



        private void writePICTTrailer(BeBinaryWriter imageOutput, int size)
        {

            // Write out end opcode. Be sure to be word-aligned.
            Int32 length = size; //imageOutput.BaseStream.Length;
            if (length == -1)
            {
                throw new IOException("Cannot write trailer without knowing length");
            }

            if ((length & 1) > 0)
            {
                imageOutput.Write((byte)0);
            }

            imageOutput.WriteShort(Pict.OP_END_OF_PICTURE);
            imageOutput.Flush();

            var offset = imageOutput.Seek(512, SeekOrigin.Begin);
            imageOutput.WriteShort((ushort)offset);

            imageOutput.Flush();

        }



    }
}
