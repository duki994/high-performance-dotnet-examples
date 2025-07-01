using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessingPipeline;

public static class ImageProcessingHelpers
{
    public static Image ResizeImage(Image image, Size newSize)
    {
        image.Mutate(x => x.Resize(newSize.Width, newSize.Height));
        return image;
    }

    public static Image ApplyGrayscaleFilter(Image image)
    {
        image.Mutate(x => x.Grayscale());
        return image;
    }

    public static void SaveImage(Image image)
    {
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(outputDir);
        
        // ensure file path has correct extension
        string filePath = Path.Combine(outputDir, $"{Guid.NewGuid()}.jpg");
        
        
        // save image using new JPEG Encoder
        image.Save(filePath);
    }
    
    
}