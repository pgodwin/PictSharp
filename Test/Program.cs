using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var original = new Bitmap("Lenna.png");
            //original.SetResolution(96, 96);
            //var image = new Bitmap(original);
            var image = original;
            var pictEncoder = new PictCodec.Pict();
            if (File.Exists("Lenna.pict"))
                File.Delete("Lenna.pict");
            using (var output = new FileStream("Lenna.pict", FileMode.CreateNew))
                pictEncoder.Encode(output, image);
        }
    }
}
