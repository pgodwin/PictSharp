<h1 align="center">
<img src="https://raw.githubusercontent.com/pgodwin/PictSharp/master/pictsharp.png" alt="PictSharp" width="256"/>
<br/>
PictSharp
</h1>


<div align="center">

[![build](https://github.com/pgodwin/PictSharp/actions/workflows/build.yml/badge.svg)](https://github.com/pgodwin/PictSharp/actions/workflows/build.yml)
[![Nuget PictSharp.Core](https://img.shields.io/nuget/v/PictSharp.Core?label=nuget%20PictSharp.Core)](https://www.nuget.org/packages/PictSharp.Core/)
[![Nuget PictSharp.ImageSharpAdaptor](https://img.shields.io/nuget/v/PictSharp.ImageSharpAdaptor?label=nuget%20PictSharp.ImageSharpAdaptor)](https://www.nuget.org/packages/PictSharp.ImageSharpAdaptor/)
[![Nuget PictSharp.Drawing](https://img.shields.io/nuget/v/PictSharp.Drawing?label=nuget%20PictSharp.Drawing)](https://www.nuget.org/packages/PictSharp.Drawing/)
![GitHub](https://img.shields.io/github/license/pgodwin/PictSharp)
</div>

### **PictSharp** is a C# Library for encoding bitmap images to to Apple's legacy [PICT Format](https://en.wikipedia.org/wiki/PICT). 

## Features

 - Implemented entirely in C# code. No native dependencies.
 - Supports .NET Framework 4.6.1+, .NET Core 3.1 and runtimes compatible with .NET Standard 2.0
 - Writes PICT 2.0 Images (so should work on a Mac II onwards with Color QuickDraw)
 - Supports 1bpp, 2bpp, 4bpp, 8bpp and 32bpp image encoding, with PackBits compression
 - Extensions available for [ImageSharp](https://github.com/SixLabors/ImageSharp) and `System.Drawing.Bitmap`

## What's not supported?
 - Vector, text, clipping, etc. This is purely for raster images.
 - 16-bit images aren't working (16-bit images use a variant of PackBits which works on WORD (16 bit) values).
 - Decoding any PICT images
 - Compression other than PackBits (ie JPEG)


## Quick Start with `ImageSharp`

📦 Install the Package from nuget

```bash
Install-Package PictSharp.ImageSharpAdaptor
```

The following will convert an ImageSharp image into PICT

```csharp
using PictSharp.ImageSharpAdaptor;
...
// Register the PICT Encoder with ImageSharp
var module = new PictConfigurationModule();
module.Configure(SixLabors.ImageSharp.Configuration.Default);

// Load an ImageSharpImage
using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>("Lenna32.png"))
{
   // Save to PICT
   image.SaveAsPict("Lenna32.pict");
}

```

You can control the bit depth of the images by setting the encoder.
```csharp
var encoder = new PictEncoder();
encoder.PictBpp = PictBpp.Bit8;
image.SaveAsPict("Lenna8bpp.pict", encoder);
```

### `System.Drawing.Bitmap` support
For .NET Framework 4.6.1+ there is support for `System.Drawing.Bitmap` under Windows.

📦Grab the package:
```bash
Install-Package PictSharp.Drawing
```

```csharp
using PictSharp.Drawing;
...

var image = new System.Drawing.Bitmap("source.png"); ;

using (var output = new FileStream("output.pict", FileMode.CreateNew))
   image.SaveAsPict(output);
```


## What is PICT?
PICT is a legacy meta-file format created by Apple, which contains (basically) serialised QuickDraw commands which can be "replayed" to produce an image. It can contain both vector and bitmap components. It is somewhat similar in concept to [Windows Metafile (WMF)](https://en.wikipedia.org/wiki/Windows_Metafile) format. The format itself is not clearly documented, with only limited documentation available from Apple, and few implementations available. 

In 2021 the most common place you'll find PICT files is MacOS Clipboard data, and [RTF documents](http://latex2rtf.sourceforge.net/RTF-Spec-1.0.txt) (\macpict). 

The format was extended with the release of QuickTime 2.0 to support QuickTime compressors, so can contain not just JPEG or PNG images, but also Video Codecs, like [Cinepak](https://en.wikipedia.org/wiki/Cinepak), [Quicktime Animation](https://en.wikipedia.org/wiki/QuickTime_Animation)/[Graphics](https://en.wikipedia.org/wiki/QuickTime_Graphics), ["Apple Video" (RDZA)](https://en.wikipedia.org/wiki/Apple_Video), etc. 

## Other Implementations

 * [ImageMagik](https://imagemagick.org/index.php) - very well knonwn library and commandline tool which supports a wide variety of formats. Includes a decoder and encoder for raster PICT images. Note that 1bpp images seem to be broken as of Jan 2022. 
 * [TwelveMonkeys ImageIO](https://github.com/haraldk/TwelveMonkeys) - JAVA library with support for a huge range of formats. PICT support is quite good and even supports some QuickDraw commands, QuickTime stills and more. I suspect it's decoding support is better than ImageMagik and even Photoshop
 * QuickTime - QuickTime has support for encoding and decoding images on Windows and MacOS. Exporting from QuickTime seems to produce "QuickTime Still" images rather than PICT images, so compatibility of exported images is not high. 
 * Adobe Photoshop - Photoshop has some limited support for PICT, both decoding and encoding. I've found that CS2 at least will display an unsupport error on some formats (like 1bpp images).

Know of some more? Please let me know!


## Credits

 - Loosely based on the [PICT encoder from TwelveMonkeys](https://github.com/haraldk/TwelveMonkeys/blob/master/imageio/imageio-pict/src/main/java/com/twelvemonkeys/imageio/plugins/pict/PICTImageWriter.java) by Harald Kuhr. 
 - 1bpp, 2bpp and 8bpp support is based on studying the [ImageMagick source](https://github.com/ImageMagick/ImageMagick/blob/main/coders/pict.c) and [Apple Quickdraw documentation](docs/). 
 - The PackBits compression is from the [C# TiffLibrary]([TiffLibrary](https://github.com/yigolden/TiffLibrary)) by [yigolden](https://github.com/yigolden).
 - Uses on [Be.IO](https://github.com/jamesqo/Be.IO) by [James Ko](https://github.com/jamesqo/) to make life easier with Big-Endian values. (This could be removed if it's an issue for your project).

## Other Resources
 - [Apple Pictures Documentation](https://developer.apple.com/library/archive/documentation/mac/QuickDraw/QuickDraw-332.html)
 - [Apple Opcodes documentation](https://developer.apple.com/library/archive/documentation/mac/QuickDraw/QuickDraw-461.html)
 - [QuickTime ImageCompression Manager](http://mirror.informatimago.com/next/developer.apple.com/documentation/QuickTime/INMAC/QT/iqImageCompMgr.1.htm)
 - [QuickTime Still Image Formats (pg 307)](https://web.archive.org/web/20010602164320/http://developer.apple.com/techpubs/quicktime/qtdevdocs/PDF/QTFileFormat.pdf)

## License
MIT License

## Questions / Support
As always, supplied as-is without any warranties. If you find an issue, please raise it via Github. PRs welcomed.

