using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictSharp
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class PictBase
    {

        /** PICT V1 identifier, two bytes, mask with 0xffff0000 */
        public const int MAGIC_V1 = 0x11010000;
        /** PICT V2 identifier, four bytes */
        public const int MAGIC_V2 = 0x001102ff;

        public const int PICT_NULL_HEADER_SIZE = 512;

        // V2 Header, -1 (int)
        public const int HEADER_V2 = -1; //unchecked((int)0xffffffff);

        // V2 Extended header, -2 (short) + reserved (short) 
        public const int HEADER_V2_EXT = unchecked((int)0xfffe0000);
        //public static short HEADER_V2_EXT = -2;

        

        // PICT/QuickDraw uses 16 bit precision per color component internally
        public const int COLOR_COMP_SIZE = 2;

        /** Default Apple Macintosh DPI setting (72 DPI). */
        public const int MAC_DEFAULT_DPI = 72;

        /**
         * PICT opcodes.
         */
        public const int OP_HEADER_OP = 0x0C00;
        public const int NOP = 0x00;
        public const int OP_CLIP_RGN = 0x01;
        public const int OP_BK_PAT = 0x02;
        public const int OP_TX_FONT = 0x03;
        public const int OP_TX_FACE = 0x04;
        public const int OP_TX_MODE = 0x05;
        public const int OP_SP_EXTRA = 0x06;
        public const int OP_PN_SIZE = 0x07;
        public const int OP_PN_MODE = 0x08;
        public const int OP_PN_PAT = 0x09;
        public const int OP_FILL_PAT = 0x0A;
        public const int OP_OV_SIZE = 0x0B;
        public const int OP_ORIGIN = 0x0C;
        public const int OP_TX_SIZE = 0x0D;
        public const int OP_FG_COLOR = 0x0E;
        public const int OP_BK_COLOR = 0x0F;
        public const int OP_TX_RATIO = 0x10;
        public static short OP_VERSION = 0x11;
        /* Not implemented */
        public const int OP_BK_PIX_PAT = 0x12;
        public const int OP_PN_PIX_PAT = 0x13;
        public const int OP_FILL_PIX_PAT = 0x14;
        public const int OP_PN_LOC_H_FRAC = 0x15;
        public const int OP_CH_EXTRA = 0x16;
        public const int OP_RGB_FG_COL = 0x1A;
        public const int OP_RGB_BK_COL = 0x1B;
        public const int OP_HILITE_MODE = 0x1C;
        public const int OP_HILITE_COLOR = 0x1D;
        public const int OP_DEF_HILITE = 0x1E;
        public const int OP_OP_COLOR = 0x1F;
        public const int OP_LINE = 0x20;
        public const int OP_LINE_FROM = 0x21;
        public const int OP_SHORT_LINE = 0x22;
        public const int OP_SHORT_LINE_FROM = 0x23;
        public const int OP_LONG_TEXT = 0x28;
        public const int OP_DH_TEXT = 0x29;
        public const int OP_DV_TEXT = 0x2A;
        public const int OP_DHDV_TEXT = 0x2B;
        public const int OP_FONT_NAME = 0x2C;
        public const int OP_LINE_JUSTIFY = 0x2D;
        public const int OP_GLYPH_STATE = 0x2E;
        public const int OP_FRAME_RECT = 0x30;
        public const int OP_PAINT_RECT = 0x31;
        public const int OP_ERASE_RECT = 0x32;
        public const int OP_INVERT_RECT = 0x33;
        public const int OP_FILL_RECT = 0x34;
        public const int OP_FRAME_SAME_RECT = 0x38;
        public const int OP_PAINT_SAME_RECT = 0x39;
        public const int OP_ERASE_SAME_RECT = 0x3A;
        public const int OP_INVERT_SAME_RECT = 0x3B;
        public const int OP_FILL_SAME_RECT = 0x3C;
        public const int OP_FRAME_R_RECT = 0x40;
        public const int OP_PAINT_R_RECT = 0x41;
        public const int OP_ERASE_R_RECT = 0x42;
        public const int OP_INVERT_R_RECT = 0x43;
        public const int OP_FILL_R_RECT = 0x44;
        public const int OP_FRAME_SAME_R_RECT = 0x48;
        public const int OP_PAINT_SAME_R_RECT = 0x49;
        public const int OP_ERASE_SAME_R_RECT = 0x4A;
        public const int OP_INVERT_SAME_R_RECT = 0x4B;
        public const int OP_FILL_SAME_R_RECT = 0x4C;
        public const int OP_FRAME_OVAL = 0x50;
        public const int OP_PAINT_OVAL = 0x51;
        public const int OP_ERASE_OVAL = 0x52;
        public const int OP_INVERT_OVAL = 0x53;
        public const int OP_FILL_OVAL = 0x54;
        public const int OP_FRAME_SAME_OVAL = 0x58;
        public const int OP_PAINT_SAME_OVAL = 0x59;
        public const int OP_ERASE_SAME_OVAL = 0x5A;
        public const int OP_INVERT_SAME_OVAL = 0x5B;
        public const int OP_FILL_SAME_OVAL = 0x5C;
        public const int OP_FRAME_ARC = 0x60;
        public const int OP_PAINT_ARC = 0x61;
        public const int OP_ERASE_ARC = 0x62;
        public const int OP_INVERT_ARC = 0x63;
        public const int OP_FILL_ARC = 0x64;
        public const int OP_FRAME_SAME_ARC = 0x68;
        public const int OP_PAINT_SAME_ARC = 0x69;
        public const int OP_ERASE_SAME_ARC = 0x6A;
        public const int OP_INVERT_SAME_ARC = 0x6B;
        public const int OP_FILL_SAME_ARC = 0x6C;
        public const int OP_FRAME_POLY = 0x70;
        public const int OP_PAINT_POLY = 0x71;
        public const int OP_ERASE_POLY = 0x72;
        public const int OP_INVERT_POLY = 0x73;
        public const int OP_FILL_POLY = 0x74;
        public const int OP_FRAME_SAME_POLY = 0x78;
        public const int OP_PAINT_SAME_POLY = 0x79;
        public const int OP_ERASE_SAME_POLY = 0x7A;
        public const int OP_INVERT_SAME_POLY = 0x7B;
        public const int OP_FILL_SAME_POLY = 0x7C;
        public const int OP_FRAME_RGN = 0x80;
        public const int OP_PAINT_RGN = 0x81;
        public const int OP_ERASE_RGN = 0x82;
        public const int OP_INVERT_RGN = 0x83;
        public const int OP_FILL_RGN = 0x84;
        public const int OP_FRAME_SAME_RGN = 0x88;
        public const int OP_PAINT_SAME_RGN = 0x89;
        public const int OP_ERASE_SAME_RGN = 0x8A;
        public const int OP_INVERT_SAME_RGN = 0x8B;
        public const int OP_FILL_SAME_RGN = 0x8C;
        /* Not implemented */
        public const int OP_BITS_RECT = 0x90;
        public const int OP_BITS_RGN = 0x91;
        public const int OP_PACK_BITS_RECT = 0x98;
        public const int OP_PACK_BITS_RGN = 0x99;
        public const int OP_DIRECT_BITS_RECT = 0x9A;
        /* Not implemented */
        public const int OP_DIRECT_BITS_RGN = 0x9B;
        public const int OP_SHORT_COMMENT = 0xA0;
        public const int OP_LONG_COMMENT = 0xA1;
        public const int OP_END_OF_PICTURE = 0xFF;
        public const int OP_VERSION_2 = 0x02FF;
        public const int OP_COMPRESSED_QUICKTIME = 0x8200;

        public const int OP_UNCOMPRESSED_QUICKTIME = 0x8201;

        public const string APPLE_USE_RESERVED_FIELD = "Reserved for Apple use.";

        /*
         * Picture comment 'kind' codes from: http://developer.apple.com/technotes/qd/qd_10.html
      public const int TextBegin = 150;
      public const int TextEnd = 151;
      public const int StringBegin = 152;
      public const int StringEnd = 153;
      public const int TextCenter = 154;
      public const int LineLayoutOff = 155;
      public const int LineLayoutOn = 156;
      public const int ClientLineLayout = 157;
      public const int PolyBegin = 160;
      public const int PolyEnd = 161;
      public const int PolyIgnore = 163;
      public const int PolySmooth = 164;
      public const int PolyClose = 165;
      public const int DashedLine = 180;
      public const int DashedStop = 181;
      public const int SetLineWidth = 182;
      public const int PostScriptBegin = 190;
      public const int PostScriptEnd = 191;
      public const int PostScriptHandle = 192;
      public const int PostScriptFile = 193;
      public const int TextIsPostScript = 194;
      public const int ResourcePS = 195;
      public const int PSBeginNoSave = 196;
      public const int SetGrayLevel = 197;
      public const int RotateBegin = 200;
      public const int RotateEnd = 201;
      public const int RotateCenter = 202;
      public const int FormsPrinting = 210;
      public const int EndFormsPrinting = 211;
      public const int ICC_Profile = 224;
      public const int Photoshop_Data = 498;
      public const int BitMapThinningOff = 1000;
      public const int BitMapThinningOn = 1001;
         */

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
