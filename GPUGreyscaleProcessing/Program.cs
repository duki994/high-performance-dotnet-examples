using System.Runtime.InteropServices;
using ILGPU;
using ILGPU.Runtime;
using IronSoftware.Drawing;

var filePath = Path.Join(Directory.GetCurrentDirectory(), "input.png");
var bitmap = new AnyBitmap(filePath, true);
var width = bitmap.Width;
var height = bitmap.Height;

// Stride --> Number of actual bytes needed to hold single row of pixels
// It may be higher than width * bytesPerPixel due to padding for specific boundary (i.e 4-byte memory boundary) 
var stride = bitmap.Stride;

var bytes = Math.Abs(stride) * height;
var pixelData = new byte[bytes];

Marshal.Copy(bitmap.Scan0, pixelData, 0, bytes);

using var context = Context.Create(builder => builder.AllAccelerators());

var device = context.GetPreferredDevice(false);
using (var accelerator = device.CreateAccelerator(context))
{
    Console.WriteLine($"Running on {accelerator.AcceleratorType} accelerator.");
    accelerator.PrintInformation();

    var inputBuffer = accelerator.Allocate1D<byte>(pixelData.Length);
    var outputBuffer = accelerator.Allocate1D<byte>(pixelData.Length);

    inputBuffer.CopyFromCPU(pixelData);

    for (var i = 0; i < 20; i++) Console.WriteLine();

    var grayscaleKernel =
        accelerator.LoadAutoGroupedStreamKernel<Index1D, ArrayView<byte>, ArrayView<byte>>(GrayscaleKernel);

    var numPixels = pixelData.Length / 4; // 4 bytes per pixel - RGBA
    grayscaleKernel(numPixels, inputBuffer.View, outputBuffer.View);

    accelerator.Synchronize();

    var result = outputBuffer.GetAsArray1D();
    // for (var i = 0; i < 20; i++)
    //     Console.WriteLine($"Grayscale pixel {i}: " +
    //                       $"{result[i * 4]}, {result[i * 4 + 1]}, {result[i * 4 + 2]}, {result[i * 4 + 3]}" +
    //                       Environment.NewLine +
    //                       $"Original pixel {i}: " +
    //                       $"{pixelData[i * 4]}, {pixelData[i * 4 + 1]}, {pixelData[i * 4 + 2]}, {pixelData[i * 4 + 3]}");

    // Diagnostic
    var min = result[0];
    var max = result[0];
    for (int i = 0; i < result.Length; i += 4)
    {
        if (result[i] < min) min = result[i];
        if (result[i] > max) max = result[i];
    }

    Console.WriteLine($"Grayscale value range: {min} - {max}");

    // Create a new bitmap and manually set each pixel.
    // This is slower but more reliable than memory copy methods that have failed.
    using (var resultBitmap = new AnyBitmap(width, height))
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var index = (y * width + x) * 4;
                var b = result[index];
                var g = result[index + 1];
                var r = result[index + 2];
                var a = result[index + 3];
                resultBitmap.SetPixel(x, y, new IronSoftware.Drawing.Color(r, g, b, a));
            }
        }

        resultBitmap.SaveAs("output_grayscale.png", AnyBitmap.ImageFormat.Png);
    }

    inputBuffer.Dispose();
    outputBuffer.Dispose();
    bitmap.Dispose(); // Dispose the original bitmap
}

Console.WriteLine("Grayscale conversion completed.");

static void GrayscaleKernel(Index1D index, ArrayView<byte> inputBuffer, ArrayView<byte> outputBuffer)
{
    int i = index * 4; // 4 components (each 1 byte) per pixel - RGBA

    var b = inputBuffer[i];
    var g = inputBuffer[i + 1];
    var r = inputBuffer[i + 2];

    // Grayscale luminance formula
    var gray = (byte)(0.299f * r + 0.587f * g + 0.114f * b);

    outputBuffer[i] = gray; // blue --> gray
    outputBuffer[i + 1] = gray; // green --> gray
    outputBuffer[i + 2] = gray; // red --> gray
    outputBuffer[i + 3] = inputBuffer[i + 3]; // alpha is the same
}