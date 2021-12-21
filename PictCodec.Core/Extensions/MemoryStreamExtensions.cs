using System.IO;


namespace PictCodec.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static void Clear(this MemoryStream stream)
        {
            stream.SetLength(0);
        }

        public static long Remaining(this MemoryStream stream)
        {
            return stream.Length - stream.Position;
        }
    }
}
