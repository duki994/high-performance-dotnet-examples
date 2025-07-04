using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessingPipeline;

public class ImagePipeline
{
    public async Task ProcessImagesAsync(IEnumerable<string> imagePaths)
    {
        // Stage 1: load images
        var loadBlock = new TransformBlock<string, Image>(path => { return Image.Load(path); },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 4
            });

        // Stage 2: resize
        var resizeBlock = new TransformBlock<Image, Image>(image =>
        {
            image.Mutate(x => x.Resize(800, 600));
            return image;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 4
        });

        // Stage 3: Apply Grayscale filter
        var filterBlock = new TransformBlock<Image, Image>(image =>
        {
            image.Mutate(x => x.Grayscale());
            return image;
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 4
        });

        // Stage 4: Save
        var saveBlock = new ActionBlock<Image>(image =>
        {
            var outputPathDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
            Directory.CreateDirectory(outputPathDir);

            var outputPath = Path.Combine(outputPathDir, $"{Guid.NewGuid()}.jpg");

            image.Save(outputPath, new JpegEncoder());
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 4
        });

        // Link the blocks

        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true
        };

        loadBlock.LinkTo(resizeBlock, linkOptions);
        resizeBlock.LinkTo(filterBlock, linkOptions);
        filterBlock.LinkTo(saveBlock, linkOptions);


        foreach (var imagePath in imagePaths) await loadBlock.SendAsync(imagePath);

        // Signal completion
        loadBlock.Complete();
        await saveBlock.Completion;
    }
}