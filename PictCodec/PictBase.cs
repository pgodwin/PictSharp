using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
{
    public class PictBase
    {

        /** PICT V1 identifier, two bytes, mask with 0xffff0000 */
        public static int MAGIC_V1 = 0x11010000;
        /** PICT V2 identifier, four bytes */
        public static int MAGIC_V2 = 0x001102ff;

        public static int PICT_NULL_HEADER_SIZE = 512;

        // V2 Header, -1 (int)
        public static int HEADER_V2 = -1; //unchecked((int)0xffffffff);

        // V2 Extended header, -2 (short) + reserved (short) 
        public static int HEADER_V2_EXT = unchecked((int)0xfffe0000);
        //public static short HEADER_V2_EXT = -2;

        // PICT/QuickDraw uses 16 bit precision per color component internally
        public static int COLOR_COMP_SIZE = 2;

        /** Default Apple Macintosh DPI setting (72 DPI). */
        public static int MAC_DEFAULT_DPI = 72;

        /**
         * PICT opcodes.
         */
        public static int OP_HEADER_OP = 0x0C00;
        public static int NOP = 0x00;
        public static int OP_CLIP_RGN = 0x01;
        public static int OP_BK_PAT = 0x02;
        public static int OP_TX_FONT = 0x03;
        public static int OP_TX_FACE = 0x04;
        public static int OP_TX_MODE = 0x05;
        public static int OP_SP_EXTRA = 0x06;
        public static int OP_PN_SIZE = 0x07;
        public static int OP_PN_MODE = 0x08;
        public static int OP_PN_PAT = 0x09;
        public static int OP_FILL_PAT = 0x0A;
        public static int OP_OV_SIZE = 0x0B;
        public static int OP_ORIGIN = 0x0C;
        public static int OP_TX_SIZE = 0x0D;
        public static int OP_FG_COLOR = 0x0E;
        public static int OP_BK_COLOR = 0x0F;
        public static int OP_TX_RATIO = 0x10;
        public static short OP_VERSION = 0x11;
        /* Not implemented */
        public static int OP_BK_PIX_PAT = 0x12;
        public static int OP_PN_PIX_PAT = 0x13;
        public static int OP_FILL_PIX_PAT = 0x14;
        public static int OP_PN_LOC_H_FRAC = 0x15;
        public static int OP_CH_EXTRA = 0x16;
        public static int OP_RGB_FG_COL = 0x1A;
        public static int OP_RGB_BK_COL = 0x1B;
        public static int OP_HILITE_MODE = 0x1C;
        public static int OP_HILITE_COLOR = 0x1D;
        public static int OP_DEF_HILITE = 0x1E;
        public static int OP_OP_COLOR = 0x1F;
        public static int OP_LINE = 0x20;
        public static int OP_LINE_FROM = 0x21;
        public static int OP_SHORT_LINE = 0x22;
        public static int OP_SHORT_LINE_FROM = 0x23;
        public static int OP_LONG_TEXT = 0x28;
        public static int OP_DH_TEXT = 0x29;
        public static int OP_DV_TEXT = 0x2A;
        public static int OP_DHDV_TEXT = 0x2B;
        public static int OP_FONT_NAME = 0x2C;
        public static int OP_LINE_JUSTIFY = 0x2D;
        public static int OP_GLYPH_STATE = 0x2E;
        public static int OP_FRAME_RECT = 0x30;
        public static int OP_PAINT_RECT = 0x31;
        public static int OP_ERASE_RECT = 0x32;
        public static int OP_INVERT_RECT = 0x33;
        public static int OP_FILL_RECT = 0x34;
        public static int OP_FRAME_SAME_RECT = 0x38;
        public static int OP_PAINT_SAME_RECT = 0x39;
        public static int OP_ERASE_SAME_RECT = 0x3A;
        public static int OP_INVERT_SAME_RECT = 0x3B;
        public static int OP_FILL_SAME_RECT = 0x3C;
        public static int OP_FRAME_R_RECT = 0x40;
        public static int OP_PAINT_R_RECT = 0x41;
        public static int OP_ERASE_R_RECT = 0x42;
        public static int OP_INVERT_R_RECT = 0x43;
        public static int OP_FILL_R_RECT = 0x44;
        public static int OP_FRAME_SAME_R_RECT = 0x48;
        public static int OP_PAINT_SAME_R_RECT = 0x49;
        public static int OP_ERASE_SAME_R_RECT = 0x4A;
        public static int OP_INVERT_SAME_R_RECT = 0x4B;
        public static int OP_FILL_SAME_R_RECT = 0x4C;
        public static int OP_FRAME_OVAL = 0x50;
        public static int OP_PAINT_OVAL = 0x51;
        public static int OP_ERASE_OVAL = 0x52;
        public static int OP_INVERT_OVAL = 0x53;
        public static int OP_FILL_OVAL = 0x54;
        public static int OP_FRAME_SAME_OVAL = 0x58;
        public static int OP_PAINT_SAME_OVAL = 0x59;
        public static int OP_ERASE_SAME_OVAL = 0x5A;
        public static int OP_INVERT_SAME_OVAL = 0x5B;
        public static int OP_FILL_SAME_OVAL = 0x5C;
        public static int OP_FRAME_ARC = 0x60;
        public static int OP_PAINT_ARC = 0x61;
        public static int OP_ERASE_ARC = 0x62;
        public static int OP_INVERT_ARC = 0x63;
        public static int OP_FILL_ARC = 0x64;
        public static int OP_FRAME_SAME_ARC = 0x68;
        public static int OP_PAINT_SAME_ARC = 0x69;
        public static int OP_ERASE_SAME_ARC = 0x6A;
        public static int OP_INVERT_SAME_ARC = 0x6B;
        public static int OP_FILL_SAME_ARC = 0x6C;
        public static int OP_FRAME_POLY = 0x70;
        public static int OP_PAINT_POLY = 0x71;
        public static int OP_ERASE_POLY = 0x72;
        public static int OP_INVERT_POLY = 0x73;
        public static int OP_FILL_POLY = 0x74;
        public static int OP_FRAME_SAME_POLY = 0x78;
        public static int OP_PAINT_SAME_POLY = 0x79;
        public static int OP_ERASE_SAME_POLY = 0x7A;
        public static int OP_INVERT_SAME_POLY = 0x7B;
        public static int OP_FILL_SAME_POLY = 0x7C;
        public static int OP_FRAME_RGN = 0x80;
        public static int OP_PAINT_RGN = 0x81;
        public static int OP_ERASE_RGN = 0x82;
        public static int OP_INVERT_RGN = 0x83;
        public static int OP_FILL_RGN = 0x84;
        public static int OP_FRAME_SAME_RGN = 0x88;
        public static int OP_PAINT_SAME_RGN = 0x89;
        public static int OP_ERASE_SAME_RGN = 0x8A;
        public static int OP_INVERT_SAME_RGN = 0x8B;
        public static int OP_FILL_SAME_RGN = 0x8C;
        /* Not implemented */
        public static int OP_BITS_RECT = 0x90;
        public static int OP_BITS_RGN = 0x91;
        public static int OP_PACK_BITS_RECT = 0x98;
        public static int OP_PACK_BITS_RGN = 0x99;
        public static int OP_DIRECT_BITS_RECT = 0x9A;
        /* Not implemented */
        public static int OP_DIRECT_BITS_RGN = 0x9B;
        public static int OP_SHORT_COMMENT = 0xA0;
        public static int OP_LONG_COMMENT = 0xA1;
        public static int OP_END_OF_PICTURE = 0xFF;
        public static int OP_VERSION_2 = 0x02FF;
        public static int OP_COMPRESSED_QUICKTIME = 0x8200;
        public static int OP_UNCOMPRESSED_QUICKTIME = 0x8201;

        public static string APPLE_USE_RESERVED_FIELD = "Reserved for Apple use.";

        /*
         * Picture comment 'kind' codes from: http://developer.apple.com/technotes/qd/qd_10.html
      public static int TextBegin = 150;
      public static int TextEnd = 151;
      public static int StringBegin = 152;
      public static int StringEnd = 153;
      public static int TextCenter = 154;
      public static int LineLayoutOff = 155;
      public static int LineLayoutOn = 156;
      public static int ClientLineLayout = 157;
      public static int PolyBegin = 160;
      public static int PolyEnd = 161;
      public static int PolyIgnore = 163;
      public static int PolySmooth = 164;
      public static int PolyClose = 165;
      public static int DashedLine = 180;
      public static int DashedStop = 181;
      public static int SetLineWidth = 182;
      public static int PostScriptBegin = 190;
      public static int PostScriptEnd = 191;
      public static int PostScriptHandle = 192;
      public static int PostScriptFile = 193;
      public static int TextIsPostScript = 194;
      public static int ResourcePS = 195;
      public static int PSBeginNoSave = 196;
      public static int SetGrayLevel = 197;
      public static int RotateBegin = 200;
      public static int RotateEnd = 201;
      public static int RotateCenter = 202;
      public static int FormsPrinting = 210;
      public static int EndFormsPrinting = 211;
      public static int ICC_Profile = 224;
      public static int Photoshop_Data = 498;
      public static int BitMapThinningOff = 1000;
      public static int BitMapThinningOn = 1001;
         */

    }
}
