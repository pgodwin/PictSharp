PictSharp
=========

[![build](https://github.com/pgodwin/PictSharp/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/pgodwin/PictSharp/actions/workflows/dotnet-build.yml)
![Nuget Core](https://img.shields.io/nuget/v/PictSharp.Core?label=nuget%20core)
![Nuget](https://img.shields.io/nuget/v/PictSharp.ImageSharpAdaptor?label=nuget%20ImageSharpAdaptor)


PictSharp is a .NET library for converting images to Apple's [PICT Format](https://en.wikipedia.org/wiki/PICT). 

It's based on PICT encoder from TwelveMonkeys by Harald Kuhr. 
8-bit support is based on studying the ImageMagick and Apple Quickdraw documentation. 

The PackBits encoding is ported from the Apache Commons Image Library Java implementation.

Implemented today is support for:
 - 32-bit (4-channel) PackBits compressed
 - Indexed 1, 2, 4 and 8bpp Images (Packbits)

All are PICT 2.0 format (basically MacII onwards).

What's not supported?
=====================
 - 16 and 24-bit images aren't working
 - Decoding any PICT images

Quick Start
============

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
   image.SaveAsPict(outName);
}

```

## License
MIT License



Questions / Support
===================
As always, supplied as-is without any warranties. If you find an issue, please raise it via Github. PRs welcomed.

