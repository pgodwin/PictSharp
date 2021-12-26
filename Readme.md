PictSharp
=========

PictSharp is a .NET library for converting images to Apple's [PICT Format](https://en.wikipedia.org/wiki/PICT). 

It's based on the 32-bit PICT encoder from TwelveMonkeys by Harald Kuhr. Indexed image support is based on studying the 
ImageMagick and Apple Quickdraw documentation.

PackBits has been ported from the TIFF PackBits implementation in ImageSharp.

## What's supported?
 - PICT 2.0 Encoding of bitmap data
 - 32-bit (4-channel) PackBits compressed
 - 2, 4 & 8-bit (1-channel) PackBits compressed 
 - 1-bit Images PackBit compressed

What's not supported?
=====================
 - Decoding any PICT images
 - 16-Bit images (more work required)

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

