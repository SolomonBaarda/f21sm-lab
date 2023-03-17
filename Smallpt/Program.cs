using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Smallpt
{
    class SmallptMain
    {
        static void Main(string[] args)
        {
            const int width = 1000;
            const int height = 700;
            string outputImagePath = Path.GetFullPath("./output.png");

            // Create stopwatch
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // Perform processing
            Image outputImage = Smallpt.RayTrace(width, height);

            // Calculate runtime
            watch.Stop();
            Console.WriteLine($"Completed processing in {watch.ElapsedMilliseconds} milliseconds");
            Console.WriteLine($"Writing output image to path {outputImagePath}");

            // Save result
            outputImage.Save(outputImagePath);
        }
    }
}
