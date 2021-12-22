using System;
using System.Collections.Generic;
using System.Text;

namespace PictCodec.Extensions
{
    public static class ArrayExtensions
    {
        public static byte[] Slice(this byte[] source, int start)
        {
            var arrayLength = source.Length;
            var length = arrayLength - start;
            byte[] dest = new byte[length];
            Array.Copy(source, start, dest, 0, length);
            return dest;            
        }

        public static byte[] Slice(this byte[] source, int start, int length)
        {
            byte[] dest = new byte[length];
            Array.Copy(source, start, dest, 0, length);
            return dest;
        }
    }
}
