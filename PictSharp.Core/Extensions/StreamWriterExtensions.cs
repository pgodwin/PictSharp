using Be.IO;
using System;


namespace PictSharp.Extensions
{
    public static class StreamExtensions
    {


        public static void WriteShort(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToInt16(value));
        }

        public static void WriteUShort(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToUInt16(value));
        }

        public static void WriteInt(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToInt32(value));
        }

        public static void WriteUInt(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToUInt32(value));
        }
    }


}
