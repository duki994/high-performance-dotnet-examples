using BenchmarkDotNet.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessingPipeline;

[MemoryDiagnoser]
public class ImageProcessingBenchmark
{
    private List<string> imagePaths;
    private ImagePipeline pipeline;

    [GlobalSetup]
    public void GlobalSetup()
    {
        imagePaths = ImageGenerator.GenerateSampleImages(10);
        pipeline = new ImagePipeline();
    }

    [Benchmark(Baseline = true)]
    public void SequentialProcessing()
    {
        foreach (var imagePath in imagePaths)
            using (var image = Image.Load<Rgba32>(imagePath))
            {
                var newSize = new Size(image.Width, image.Height);
                using (var resized = ImageProcessingHelpers.ResizeImage(image, newSize))
                {
                    using (var filtered = ImageProcessingHelpers.ApplyGrayscaleFilter(resized))
                    {
                        ImageProcessingHelpers.SaveImage(filtered);
                    }
                }
            }
    }

    [Benchmark]
    public async Task PipelinedProcessing()
    {
        await pipeline.ProcessImagesAsync(imagePaths);
    }
}