using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SobelEdgeDetector
{
    class SobelEdgeDetectorMain
    {
        static void Main(string[] args)
        {
            // Ensure that we have the correct command line arguments
            if (args.Length != 4)
            {
                throw new ArgumentException("Edge detector must be called with <task number> <input image path> <output image path> <number of threads>");
            }

            // Parse parameters
            SobelEdgeDetector.LabTask task = (SobelEdgeDetector.LabTask)(int.Parse(args[0]));
            string inputImagePath = Path.GetFullPath(args[1]);
            string outputImagePath = Path.GetFullPath(args[2]);
            int numberOfThreads = int.Parse(args[3]);

            // Load image
            Console.WriteLine($"Attempting to load image from path {inputImagePath}");
            using Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath);
            Console.WriteLine($"Your machine has {Environment.ProcessorCount} logical processors");
            Console.WriteLine($"Running lab {task} with {numberOfThreads} threads...");

            // Create stopwatch
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Perform processing
            Image outputImage = SobelEdgeDetector.PerformEdgeProcessing(image, task, numberOfThreads);

            // Calculate runtime
            watch.Stop();
            Console.WriteLine($"Completed processing in {watch.ElapsedMilliseconds} milliseconds using {numberOfThreads} threads");
            Console.WriteLine($"Writing output image to path {outputImagePath}");

            // Save result
            outputImage.Save(outputImagePath);
        }
    }
}
