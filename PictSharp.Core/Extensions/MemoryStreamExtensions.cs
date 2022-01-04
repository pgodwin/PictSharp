using System.IO;


namespace PictSharp.Extensions
{
    /// <summary>
    /// MemoryStream extension
    /// </summary>
    public static class MemoryStreamExtensions
    {
        /// <summary>
        /// Sets the length of the memory stream back to 0, effectively erasing it.
        /// </summary>
        /// <param name="stream"></param>
        public static void Clear(this MemoryStream stream)
        {
            stream.SetLength(0);
        }

        /// <summary>
        /// Returns the remaining bytes in the array, assuming length and position are set correctly
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static long Remaining(this MemoryStream stream)
        {
            return stream.Length - stream.Position;
        }
    }
}
