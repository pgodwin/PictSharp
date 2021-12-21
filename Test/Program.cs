
using System;
using System.Drawing;
using System.IO;
using PictCodec;
using PictCodec.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.Formats;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = new string[] { "Lenna32.png", "Lenna24.png", "Lenna8.png", "Lenna4.png", "Lenna2.png", "Lenna1.png", "lena_gray.bmp" };
            foreach (var file in files)
            {
                try
                {

                    var original = new Bitmap(file);
                    var image = original;
                    var outName = Path.GetFileNameWithoutExtension(file) + "_drawing.pict";

                    if (File.Exists(outName))
                        File.Delete(outName);
                    using (var output = new FileStream(outName, FileMode.CreateNew))
                        image.SaveAsPict(output);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{file} failed {ex}");
                }
            }

            var module = new PictCodec.ImageSharp.PictConfigurationModule();
            module.Configure(SixLabors.ImageSharp.Configuration.Default);
           
            // Test ImageSharp
            foreach (var file in files)
            {
                try
                {
                    
                    var info = SixLabors.ImageSharp.Image.Identify(file);
                    
                    using (var image = SixLabors.ImageSharp.Image.Load(file))
                    {
                        var outName = Path.GetFileNameWithoutExtension(file) + "_imageSharp.pict";
                        image.SaveAsPict(outName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{file} failed {ex}");
                }
            }
        }
    }
}
