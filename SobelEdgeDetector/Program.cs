using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SobelEdgeDetector
{
    class SobelEdgeDetectorMain
    {
        static void Main(string[] args)
        {
            // Ensure that we have the correct command line arguments
            if (args.Length != 3)
            {
                throw new ArgumentException("Edge detector must be called with <number of threads> <input image path> <output image path>");
            }

            uint numberOfThreads = uint.Parse(args[0]);
            string inputImagePath = Path.GetFullPath(args[1]);
            string outputImagePath = Path.GetFullPath(args[2]);

            Console.WriteLine($"Attempting to load image from path {inputImagePath}");

            using Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath);

            Console.WriteLine($"Processing image using {numberOfThreads} threads");

            // Create stopwatch
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Perform processing
            Image outputImage = SobelEdgeDetector.PerformEdgeProcessing(image);

            // Calculate runtime
            watch.Stop();
            var durationMilliseconds = watch.ElapsedMilliseconds;
            Console.WriteLine($"Completed processing in {durationMilliseconds} milliseconds using {numberOfThreads} threads");
            Console.WriteLine($"Writing output image to path {outputImagePath}");

            // Save result
            outputImage.Save(outputImagePath);
        }
    }
}
