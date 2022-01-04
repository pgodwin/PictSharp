# PictSharp

[![build](https://github.com/pgodwin/PictSharp/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/pgodwin/PictSharp/actions/workflows/dotnet-build.yml)
![Nuget Core](https://img.shields.io/nuget/v/PictSharp.Core?label=nuget%20core)
![Nuget](https://img.shields.io/nuget/v/PictSharp.ImageSharpAdaptor?label=nuget%20ImageSharpAdaptor)


PictSharp is a .NET library for converting images to Apple's [PICT Format](https://en.wikipedia.org/wiki/PICT). 

Loosely based on the [PICT encoder from TwelveMonkeys](https://github.com/haraldk/TwelveMonkeys/blob/master/imageio/imageio-pict/src/main/java/com/twelvemonkeys/imageio/plugins/pict/PICTImageWriter.java) by Harald Kuhr. 

1-bpp-8-bpp support is based on studying the ImageMagick and Apple Quickdraw documentation. 

The PackBits compression is from the [C# TiffLibrary]([TiffLibrary](https://github.com/yigolden/TiffLibrary)) by yigolden.

## Supported Types:
 - 32-bit (4-channel) PackBits compressed
 - Indexed 1, 2, 4 and 8bpp Images (Packbits)

All are PICT 2.0 format (basically MacII onwards).

## What's not supported?

 - 16-bit images aren't working
 - Decoding any PICT images

## Quick Start

To use with ImageSharp, install the Package

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

You can control the bit depth of the images by setting the encoder
```csharp
var encoder = new PictEncoder();
encoder.PictBpp = PictBpp.Bit8;
image.SaveAsPict("Lenna8bpp.pict", encoder);
```

## License
MIT License

## Questions / Support
As always, supplied as-is without any warranties. If you find an issue, please raise it via Github. PRs welcomed.

