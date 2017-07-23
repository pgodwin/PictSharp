using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictCodec
{
    public interface IDecoder
    {

        bool CanDecodeSource(Stream source);

        Bitmap Decode(Stream source);
        
    }
}
