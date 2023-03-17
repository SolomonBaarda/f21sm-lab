#define PARALLEL_FOR

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SobelEdgeDetector
{
    public static class SobelEdgeDetector
    {
        // Gaussian noise reduction 5x5 kernel
        public static readonly float[,] GaussianKernel5x5 =
            {
                { 0.003f, 0.013f, 0.022f, 0.013f, 0.003f },
                { 0.013f, 0.059f, 0.097f, 0.059f, 0.013f },
                { 0.022f, 0.097f, 0.159f, 0.097f, 0.022f },
                { 0.013f, 0.059f, 0.097f, 0.059f, 0.013f },
                { 0.003f, 0.013f, 0.022f, 0.013f, 0.003f },
            };

        // Sobel 5x5 kernels
        public static readonly float[,] SobelKernelHorizontal5x5 =
            {
                { +2, +1, +0, -1, -2 },
                { +2, +1, +0, -1, -2 },
                { +4, +2, +0, -2, -4 },
                { +2, +1, +0, -1, -2 },
                { +2, +1, +0, -1, -2 },
            };

        public static readonly float[,] SobelKernelVertical5x5 =
            {
                { +2, +2, +4, +2, +2 },
                { +1, +1, +2, +1, +1 },
                { +0, +0, +0, +0, +0 },
                { -1, -1, -2, -1, -1 },
                { -2, -2, -4, -2, -2 },
            };

        public enum LabTask
        {
            Task1,
            Task2,
            Task3,
            Task4,
            Task5
        }

        public static Image<Rgba32> PerformEdgeProcessing(in Image<Rgba32> image, LabTask labTask)
        {
            // Copy raw pixel data into an array
            Rgba32[] pixelData = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixelData);

            // Perform noise reduction
            Rgba32[] blurredPixelData = Convolve(pixelData, image.Width, image.Height, GaussianKernel5x5, labTask);

            // Convert to greyscale
            Rgba32[] greyscalePixelData = new Rgba32[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                // Calculate greyscale using weighted luminosity method
                float average = ((0.3f * blurredPixelData[i].R) + (0.59f * blurredPixelData[i].G) + (0.11f * blurredPixelData[i].B)) / 256.0f;
                greyscalePixelData[i] = new Rgba32(average, average, average);
            }

            // Gradient calculation
            Rgba32[] sobelPixelDataHorizontal = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelHorizontal5x5, labTask);
            Rgba32[] sobelPixelDataVertical = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelVertical5x5, labTask);

            Rgba32[] sobelMagnitude = new Rgba32[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                sobelMagnitude[i] = new Rgba32
                (
                    (sobelPixelDataHorizontal[i].R + sobelPixelDataVertical[i].R) / 256.0f,
                    (sobelPixelDataHorizontal[i].G + sobelPixelDataVertical[i].G) / 256.0f,
                    (sobelPixelDataHorizontal[i].B + sobelPixelDataVertical[i].B) / 256.0f
                );
            }

            // Edge thinning
            // Task 6

            // Task 7
            // Output the original image with the edges on top
            // Optionally colour the edges

            // Return the processed image
            return Image<Rgba32>.LoadPixelData(sobelMagnitude, image.Width, image.Height);
        }

        private static Rgba32[] Convolve(Rgba32[] pixelData, int width, int height, float[,] kernel, LabTask labTask)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];
            int numberOfProcessors = Environment.ProcessorCount;

            switch (labTask)
            {
                case LabTask.Task1:

                    // Sequential implementation
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = (y * width) + x;
                            outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                        }
                    }

                    break;

                case LabTask.Task2:

                    // Parallel for implementation

                    Parallel.For(0, height, y =>
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = (y * width) + x;
                            outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                        }
                    });

                    // Your code goes here:
                    //throw new NotImplementedException("Please complete task 2");

                    break;

                case LabTask.Task3:

                    // Parallel LINQ implementation

                    outputPixelData = pixelData.AsParallel()
                    .AsOrdered()
                    .Select((element, index) =>
                    {
                        int x = index % width;
                        int y = index / width;
                        return ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                    })
                    .ToArray();

                    // Your code goes here:
                    //throw new NotImplementedException("Please complete task 3");

                    break;

                case LabTask.Task4:


                    // Chunked threads approach

                    List<Thread> threads = new List<Thread>();
                    int workPerProcess = pixelData.Length / numberOfProcessors;

                    for (int process = 0; process < numberOfProcessors; process++)
                    {
                        // Calculate start and end indexes
                        int startIndex = process * workPerProcess;
                        int endIndex = (process == numberOfProcessors - 1) ? pixelData.Length : startIndex + workPerProcess;

                        // Assign work to the thread
                        threads.Add(new Thread(() =>
                        {
                            // Your code goes here:

                            throw new NotImplementedException("Please complete task 4");

                            for (int index = startIndex; index < endIndex; index++)
                            {
                                int x = index % width;
                                int y = index / width;
                                outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                            }
                        }));
                    }

                    foreach (var thread in threads) thread.Start();
                    foreach (var thread in threads) thread.Join();

                    break;

                case LabTask.Task5:

                    // Chunked threads using fully dynamic partitioning

                    // Your code goes here:
                    // Look at the CountingPrimes project as a starting point
                    throw new NotImplementedException("Please complete task 5");

                    break;
            }

            // Return the new pixel data
            return outputPixelData;
        }

        static Rgba32 ApplyKernelToPixel(Rgba32[] pixelData, int width, int height, int pixelX, int pixelY, in float[,] kernel)
        {
            float sumR = 0, sumG = 0, sumB = 0;

            for (int kernelY = 0; kernelY < kernel.GetLength(0); kernelY++)
            {
                for (int kernelX = 0; kernelX < kernel.GetLength(1); kernelX++)
                {
                    // Calculate the pixel position to sample
                    // If the sample point is outside the image, just sample the closest pixel along the edge
                    int sampleY = Math.Clamp(pixelY + kernelY - (kernel.GetLength(0) / 2), 0, height - 1);
                    int sampleX = Math.Clamp(pixelX + kernelX - (kernel.GetLength(1) / 2), 0, width - 1);

                    Rgba32 samplePixel = pixelData[(sampleY * width) + sampleX];
                    // Calculate the new values using the weightings in the kernel
                    sumR += samplePixel.R * kernel[kernelY, kernelX];
                    sumG += samplePixel.G * kernel[kernelY, kernelX];
                    sumB += samplePixel.B * kernel[kernelY, kernelX];
                }
            }

            // Rgba32 takes colour input in range 0-1
            return new Rgba32(sumR / 256.0f, sumG / 256.0f, sumB / 256.0f);
        }

    }
}
