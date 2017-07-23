PictSharp
=========

PictSharp is a .NET library for converting images to Apple's [PICT Format](https://en.wikipedia.org/wiki/PICT). 

It's based on PICT encoder from TwelveMonkeys by Harald Kuhr. 
8-bit support is based on studying the ImageMagick and Apple Quickdraw documentation. 

The PackBits encoding is ported from the Apache Commons Image Library Java implementation.

Implemented today is support for:
 - 32-bit (4-channel) PackBits compressed
 - 8-bit (1-channel) PackBits compressed 

All are PICT 2.0 format (basically MacII onwards).

What's not supported?
=====================
 - 16 and 24-bit images aren't working
 - 1 and 4-bit images are probably not working.
 - Decoding any PICT images

Usage
=====
Compile the project and it to your own project. The following example loads a png file and saves it as a PICT file.

```CSharp
var image = new Bitmap("Lenna.png");
var pictEncoder = new PictCodec.Pict();
using (var output = new FileStream("Lenna.pict", FileMode.CreateNew))
    pictEncoder.Encode(output, image);
```

Questions / Support
===================
As always, supplied as-is without any warranties. If you find an issue, please raise it via Github. PRs welcomed.

