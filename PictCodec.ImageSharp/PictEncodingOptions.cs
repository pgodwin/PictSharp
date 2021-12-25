using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Collections.Generic;
using System.Text;

namespace PictSharp.ImageSharpAdaptor
{
    public class PictEncodingOptions : IPictEncodingOptions
    {
        // <summary>
        /// Initializes a new instance of the <see cref="PictEncodingOptions"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public PictEncodingOptions(IPictEncodingOptions source)
        {
            this.PictBpp = source.PictBpp;
            this.Quantizer = source.Quantizer;
            //this.IsIndexed = source.IsIndexed;
        }


        /// <inheritdoc/>
        public IQuantizer Quantizer { get; set;}

        /// <inheritdoc/>
        public PictBpp PictBpp { get; set; }

        public bool IsIndexed  => (int)PictBpp <= 8;
    }
}
