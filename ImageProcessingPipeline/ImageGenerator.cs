using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessingPipeline;

public static class ImageGenerator
{
    public static List<string> GenerateSampleImages(int count)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
        Directory.CreateDirectory(dir);

        var paths = new List<string>();

        for (var i = 0; i < count; i++)
        {
            var filePath = Path.Combine(dir, $"image_{i}.jpg");
            CreateSampleImage(filePath, i);
            paths.Add(filePath);
        }

        return paths;
    }

    private static void CreateSampleImage(string path, int seed)
    {
        var width = 1024;
        var height = 768;

        using (var image = new Image<Rgba32>(width, height))
        {
            var random = new Random(seed);

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var r = (byte)random.Next(256);
                var g = (byte)random.Next(256);
                var b = (byte)random.Next(256);
                image[x, y] = new Rgba32(r, g, b);
            }

            // save as JPEG in a valid format
            image.Save(path, new JpegEncoder());
        }
    }
}