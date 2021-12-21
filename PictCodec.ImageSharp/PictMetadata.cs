using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace PictCodec.ImageSharp
{
    public class PictMetadata : IDeepCloneable
    {
        public PictMetadata()
        {

        }

        public PictMetadata(PictMetadata other)
        {

        }

        public IDeepCloneable DeepClone() => new PictMetadata(this);
    }
}
