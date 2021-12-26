
using System;
using System.Drawing;
using System.IO;
using PictSharp.ImageSharpAdaptor;
using PictSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
        //    var files = new string[] { "Lenna32.png","Lenna8.png", "Lenna8bit.png", "lenna_gray.bmp" };
        //    foreach (var file in files)
        //    {
        //        //try
        //        //{

        //            var image = new System.Drawing.Bitmap(file); ;
        //            var outName = Path.GetFileNameWithoutExtension(file) + "_drawing.pict";

        //            if (File.Exists(outName))
        //                File.Delete(outName);
                    
        //            using (var output = new FileStream(outName, FileMode.CreateNew))
        //                image.SaveAsPict(output);

        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    Console.WriteLine($"{file} failed {ex}");
        //        //}
        //    }

            var module = new PictConfigurationModule();
            module.Configure(SixLabors.ImageSharp.Configuration.Default);

            // Test ImageSharp

            using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>("Lenna32.png"))
            {
                foreach (PictBpp bpp in Enum.GetValues(typeof(PictBpp)))
                {

                    var outName = "Lenna" + bpp.ToString() + "_imageSharp.pict";
                    var encoder = new PictEncoder();
                    //if ((int)bpp < (int)PictBpp.Bit16)
                    //    encoder.IsIndexed = true;

                    encoder.PictBpp = bpp;
                    image.SaveAsPict(outName, encoder);
                }
            }


        }
    }
}
