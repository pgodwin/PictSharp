using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
{
    public interface IEncoder
    {
        void Encode(Stream output, ImageDetails image);
    }
}
