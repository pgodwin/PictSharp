using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PictSharp
{
    /// <summary>
    /// PICT Encoder interface
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Saves the Bitmap as a PICT image
        /// </summary>
        /// <param name="image">Source Image Details</param>
        /// <param name="output">Output stream to write to</param>
        void Encode(Stream output, ImageDetails image);

        /// <summary>
        /// Encodes the image to the specified output
        /// </summary>
        /// <param name="output">Stream to output the image to</param>
        /// <param name="image">Source image details</param>
        /// <param name="cancellationToken">Cancellation token for async operations</param>
        void Encode(Stream output, ImageDetails image, CancellationToken cancellationToken);
    }
}
