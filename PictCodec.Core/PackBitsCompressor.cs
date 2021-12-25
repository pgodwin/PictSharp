using PictSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PictSharp
{
    /// <summary>
    /// https://github.com/yigolden/TiffLibrary/blob/main/src/TiffLibrary/Compression/PackBitsCompressionAlgorithm.cs
    /// </summary>
    internal class PackBitsCompressor
    {
        internal static byte[] PackBits(byte[] row)
        {
            byte[] inputSpan = new byte[row.Length];
            Array.Copy(row, inputSpan, row.Length);
            MemoryStream outputWriter = new MemoryStream(inputSpan.Length * 2);
            int pos = 0;

            //while (pos < row.Length)
            while (inputSpan.Length > 0)
            {
                // Literal bytes of data
                int literalRunLength = 1;
                for (int i = 1; i < inputSpan.Length; i++)
                {
                    if (inputSpan[i] != inputSpan[i - 1] || i == 1)
                    {
                        // Noop
                    }
                    else if (inputSpan[i] == inputSpan[i - 2])
                    {
                        literalRunLength -= 2;
                        break;
                    }
                    literalRunLength++;
                }
                if (literalRunLength > 0)
                {
                    if (literalRunLength > 128)
                    {
                        literalRunLength = 128;
                    }
                    pos += literalRunLength;
                    outputWriter.WriteByte((byte)(literalRunLength - 1));
                    outputWriter.Write(inputSpan.Slice(0, literalRunLength), 0, literalRunLength);
                    inputSpan = inputSpan.Slice(literalRunLength);
                    continue;
                }

                // Repeated bytes
                int repeatedLength = 1;
                for (int i = 1; i < inputSpan.Length; i++)
                { 
                    if (inputSpan[i] == inputSpan[0])
                    {
                        repeatedLength++;
                        pos++;
                        if (repeatedLength == 128)
                        {
                            break;
                        }
                        
                    }
                    else
                    {
                        break;
                    }
                }
                outputWriter.WriteByte((byte)(sbyte)(1 - repeatedLength));
                outputWriter.WriteByte(inputSpan[0]);
                pos += 2;
                inputSpan = inputSpan.Slice(repeatedLength);
            }

            return outputWriter.ToArray();
        }
    }

}
