using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessingPipeline;

public static class ImageGenerator
{
    public static List<string> GenerateSampleImages(int count)
    {
        string dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
        Directory.CreateDirectory(dir);
        
        var paths = new List<string>();

        for (int i = 0; i < count; i++)
        {
            string filePath = Path.Combine(dir, $"image_{i}.jpg");
            CreateSampleImage(filePath, i);
            paths.Add(filePath);
        }
        
        return paths;
    }

    private static void CreateSampleImage(string path, int seed)
    {
        int width = 1024;
        int height = 768;

        using (var image = new Image<Rgba32>(width, height))
        {
            var random = new Random(seed);

            for (int y = 0; y < height; y++)
            {

                for (int x = 0; x < width; x++)
                {
                    byte r = (byte)random.Next(256);
                    byte g = (byte)random.Next(256);
                    byte b = (byte)random.Next(256); 
                    image[x, y] = new Rgba32(r, g, b);
                }
            }
            
            // save as JPEG in a valid format
            image.Save(path, new JpegEncoder());
        }
    }
}