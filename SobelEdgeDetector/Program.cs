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
                throw new ArgumentException("Edge detector must be called with <task number> <input image path> <output image path>");
            }

            uint taskNumber = uint.Parse(args[0]);
            string inputImagePath = Path.GetFullPath(args[1]);
            string outputImagePath = Path.GetFullPath(args[2]);

            Console.WriteLine($"Attempting to load image from path {inputImagePath}");
            using Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath);

            SobelEdgeDetector.LabTask task = SobelEdgeDetector.LabTask.Task1;
            switch (taskNumber)
            {
                case 2U: task = SobelEdgeDetector.LabTask.Task2; break;
                case 3U: task = SobelEdgeDetector.LabTask.Task3; break;
                case 4U: task = SobelEdgeDetector.LabTask.Task4; break;
                case 5U: task = SobelEdgeDetector.LabTask.Task5; break;
            }

            Console.WriteLine($"Running lab {task}...");

            // Create stopwatch
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Perform processing
            Image outputImage = SobelEdgeDetector.PerformEdgeProcessing(image, task);

            // Calculate runtime
            watch.Stop();
            Console.WriteLine($"Completed processing in {watch.ElapsedMilliseconds} milliseconds using {Environment.ProcessorCount} threads");
            Console.WriteLine($"Writing output image to path {outputImagePath}");

            // Save result
            outputImage.Save(outputImagePath);
        }
    }
}
