using System;
using System.Collections.Generic;
using System.Text;

namespace PictSharp.Extensions
{
    /// <summary>
    /// Contains extensions for array types (mostly byte[])
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a new array from a slice out of the current array that begins at a specified index..
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <returns>A new array containing the just the items in the slice.</returns>
        public static byte[] Slice(this byte[] source, int start)
        {
            var arrayLength = source.Length;
            var length = arrayLength - start;
            byte[] dest = new byte[length];
            Array.Copy(source, start, dest, 0, length);
            return dest;
        }

        /// <summary>
        /// Retunrs a new array of the specified array starting at a specified index for a specified length.
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="start">The index at which to begin the slice.</param>
        /// <param name="length">The desired length for the slice.</param>
        /// <returns>A new byte[] array with a copy of the items in the range</returns>
        public static byte[] Slice(this byte[] source, int start, int length)
        {
            byte[] dest = new byte[length];
            Array.Copy(source, start, dest, 0, length);
            return dest;
        }
    }
}
