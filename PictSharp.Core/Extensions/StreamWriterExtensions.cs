using Be.IO;
using System;


namespace PictSharp.Extensions
{
    /// <summary>
    /// Contains extensions for BeBinaryWriter
    /// </summary>
    public static class StreamExtensions
    {


        /// <summary>
        /// Writes an Int16 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteShort(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToInt16(value));
        }

        /// <summary>
        /// Writes an unsigned Int16 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteUShort(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToUInt16(value));
        }

        /// <summary>
        /// Writes a signed int32 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteInt(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToInt32(value));
        }

        /// <summary>
        /// Writes an unsigned int32 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteUInt(this BeBinaryWriter writer, object value)
        {
            writer.Write(Convert.ToUInt32(value));
        }
    }


}
