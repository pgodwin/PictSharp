// Copyright (c) Peter Godwin. 
// Based on PngEncoder from ImageSharp, Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces;
using System.Threading.Tasks;

namespace PictSharp.ImageSharpAdaptor
{
    internal class PictEncoderCore : IImageEncoderInternals, IDisposable
    {

        public readonly MemoryAllocator memoryAllocator;

        private readonly Configuration configuration;

        private readonly PictEncodingOptions options;

    
        private int width;
        private int height;

        /// <summary>
        /// Initializes a new instance of the <see cref="PngEncoderCore" /> class.
        /// </summary>
        /// <param name="memoryAllocator">The <see cref="MemoryAllocator" /> to use for buffer allocations.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="options">The options for influencing the encoder</param>
        public PictEncoderCore(MemoryAllocator memoryAllocator, Configuration configuration, PictEncodingOptions options)
        {
            this.memoryAllocator = memoryAllocator;
            this.configuration = configuration;
            this.options = options;
        }


        /// <inheritdoc />
        public void Dispose()
        {

        }


        /// <summary>
        /// Encodes the image to the specified stream from the <see cref="Image{TPixel}"/>.
        /// </summary>
        /// <typeparam name="TPixel">The pixel format.</typeparam>
        /// <param name="image">The <see cref="ImageFrame{TPixel}"/> to encode from.</param>
        /// <param name="stream">The <see cref="Stream"/> to encode the image data to.</param>
        /// <param name="cancellationToken">The token to request cancellation.</param>
        public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
                where TPixel : unmanaged, IPixel<TPixel>
        {

            ImageMetadata metadata = image.Metadata;
            IndexedImageFrame<TPixel>? quantized = default; 
            PictSharp.PaletteEntry[] palette;
            ImageDetails imageDetails;
            

            this.width = image.Width;
            this.height = image.Height;


#pragma warning disable CS8604 // Possible null reference argument.
            Func<int, byte[]> getScanLine = new Func<int, byte[]>(y => this.GetScanLine(image, quantized, y).ToArray() );
#pragma warning restore CS8604 // Possible null reference argument.


            if (options.IsIndexed)
            {
                quantized = this.CreateQuantizedImage(image);
                palette = this.GetPalette(quantized);
                imageDetails = new ImageDetails(
                    this.width,
                    this.height,
                    (uint)this.options.PictBpp,
                    metadata.HorizontalResolution,
                    metadata.VerticalResolution,
                    palette,
                    getScanLine);
            }
            else
            {
                imageDetails = new ImageDetails(
                    this.width,
                    this.height,
                    (uint)this.options.PictBpp,
                    (uint) (((int)this.options.PictBpp) < 32 ? 3 : 4), // todo calculate this
                    metadata.HorizontalResolution,
                    metadata.VerticalResolution,
                    getScanLine);
            }

            PictSharp.Pict encoder = new Pict();
            encoder.Encode(stream, imageDetails, cancellationToken);
            stream.Flush();

            quantized?.Dispose();
        }

        /// <summary>
        /// Returns a PaletteEntry array from a quantized image
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="quantized">Quantized image</param>
        /// <returns>Array of Palette Entries</returns>
        private PaletteEntry[] GetPalette<TPixel>(IndexedImageFrame<TPixel> quantized) where TPixel : unmanaged, IPixel<TPixel>
        {
            ReadOnlySpan<TPixel> sourcePalette = quantized.Palette.Span;
            int paletteLength = sourcePalette.Length;
            PaletteEntry[] destinationPalette = new PaletteEntry[paletteLength];
            for (int i = 0; i < paletteLength; i++)
            {
                Rgba32 rgba = new Rgba32();
                sourcePalette[i].ToRgba32(ref rgba);
                destinationPalette[i] = new PaletteEntry(rgba.A, rgba.R, rgba.G, rgba.B);
            }
            return destinationPalette;
        }

        /// <summary>
        /// Returns a span of bytes in the correct format for the PictEncoder
        /// </summary>
        /// <typeparam name="TPixel"></typeparam>
        /// <param name="image"></param>
        /// <param name="quantized"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private ReadOnlySpan<byte> GetScanLine<TPixel>(Image<TPixel> image, IndexedImageFrame<TPixel> quantized, int y) where TPixel : unmanaged, IPixel<TPixel>
        {
            if (quantized == null && options.IsIndexed)
                throw new Exception("Quantized image expected for indexed images");

            if (quantized != null)
            {
                return quantized.GetPixelRowSpan(y);
            }
            
            var pixels = image.GetPixelRowSpan(y);
            var pixelBytes = (int)options.PictBpp / 8;
            using IMemoryOwner<byte> row = memoryAllocator.Allocate<byte>(pixelBytes * image.Width);
            Span<byte> rowSpan = row.Memory.Span;
            switch (options.PictBpp)
            {
                case PictBpp.Bit16:
                    PixelOperations<TPixel>.Instance.ToBgra5551Bytes(configuration, pixels, rowSpan, pixels.Length);
                    break;
                case PictBpp.Bit32:
                default:
                    PixelOperations<TPixel>.Instance.ToBgra32Bytes(configuration, pixels, rowSpan, pixels.Length);
                    break;
            }
            return rowSpan;
        }

        /// <summary>
        /// Creates the quantized image and sets calculates and sets the bit depth.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="image">The image to quantize.</param>
        /// <returns>The quantized image.</returns>
        private IndexedImageFrame<TPixel> CreateQuantizedImage<TPixel>(Image<TPixel> image)
            where TPixel : unmanaged, IPixel<TPixel>
        {

            // Use the metadata to determine what quantization depth to use if no quantizer has been set.
            if (options.Quantizer is null)
            {
                byte bits = (byte)options.PictBpp;
                var maxColors = GetColorCountForBitDepth(bits);
                options.Quantizer = new WuQuantizer(new QuantizerOptions { MaxColors = maxColors });
                
            }

            // Create quantized frame returning the palette and set the bit depth.
            using IQuantizer<TPixel> frameQuantizer = options.Quantizer.CreatePixelSpecificQuantizer<TPixel>(image.GetConfiguration());
            ImageFrame<TPixel> frame = image.Frames.RootFrame;
            return frameQuantizer.BuildPaletteAndQuantizeFrame(frame, frame.Bounds());

        }




        #region Internal Methods to use

        /// <summary>
        /// Returns how many colors will be created by the specified number of bits.
        /// </summary>
        /// <param name="bitDepth">The bit depth.</param>
        /// <returns>The <see cref="int"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetColorCountForBitDepth(int bitDepth)
                => 1 << bitDepth;
        #endregion
    }
}
