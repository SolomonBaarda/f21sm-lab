using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SobelEdgeDetectorSolutions
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
            Task1 = 1,
            Task2 = 2,
            Task3 = 3,
            Task4 = 4,
            Task5 = 5,
            Task6 = 6
        }


        /// <summary>
        /// The main image processing function which performs the Sobel edge detection
        /// </summary>
        /// <param name="image">Main input image</param>
        /// <param name="labTask">Current lab task to run</param>
        /// <param name="numberOfThreads">Maximum number of threads to use for processing</param>
        /// <returns>The output image containing the detected edges</returns>
        public static Image<Rgba32> PerformEdgeProcessing(in Image<Rgba32> image, LabTask labTask, int numberOfThreads)
        {
            // Copy raw pixel data into an array
            Rgba32[] pixelData = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixelData);

            // Perform noise reduction
            Rgba32[] blurredPixelData = Convolve(pixelData, image.Width, image.Height, GaussianKernel5x5, labTask, numberOfThreads);

            // Convert to greyscale
            Rgba32[] greyscalePixelData = new Rgba32[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                // Calculate greyscale using weighted luminosity method
                float average = ((0.3f * blurredPixelData[i].R) + (0.59f * blurredPixelData[i].G) + (0.11f * blurredPixelData[i].B)) / 256.0f;
                greyscalePixelData[i] = new Rgba32(average, average, average);
            }

            // Sobel gradient calculation
            Rgba32[] sobelPixelDataHorizontal = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelHorizontal5x5, labTask, numberOfThreads);
            Rgba32[] sobelPixelDataVertical = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelVertical5x5, labTask, numberOfThreads);

            // Sobel magnitude calculation
            // Determine the thickness of each of the edges
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

            // Task 7
            // Output the original image with the edges on top
            // Optionally colour the edges

            // Return the processed image
            return Image<Rgba32>.LoadPixelData(sobelMagnitude, image.Width, image.Height);
        }

        /// <summary>
        /// The main function requiring parallelization 
        /// </summary>
        /// <param name="pixelData">Input image</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="kernel">The kernel to apply to the input image</param>
        /// <param name="labTask">The current lab task to run</param>
        /// <param name="numberOfThreads">The maximum number of threads to use for processing</param>
        /// <returns>The output image for this stage in the algorithm</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Rgba32[] Convolve(Rgba32[] pixelData, int width, int height, float[,] kernel, LabTask labTask, int numberOfThreads)
        {
            // Run a different parallelization technique depending on the current lab task
            switch (labTask)
            {
                case LabTask.Task2: return Task2(pixelData, width, height, kernel, numberOfThreads);

                case LabTask.Task3: return Task3(pixelData, width, height, kernel, numberOfThreads);

                case LabTask.Task4: return Task4(pixelData, width, height, kernel, numberOfThreads);

                case LabTask.Task5: return Task5(pixelData, width, height, kernel, numberOfThreads);

                case LabTask.Task6: return Task6(pixelData, width, height, kernel, numberOfThreads);

                // Run the sequential implementation by default
                default: return Task1(pixelData, width, height, kernel, numberOfThreads);
            }
        }

        // Lab tasks
        // Your code goes here:

        // Sequential implementation
        private static Rgba32[] Task1(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;
                    outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                }
            }

            return outputPixelData;
        }

        // Parallel for implementation
        private static Rgba32[] Task2(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

                    // Your code goes here:

            Parallel.For(0, height, new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;
                    outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                }
            });

            return outputPixelData;
        }

        // Parallel LINQ implementation
        private static Rgba32[] Task3(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

            // Your code goes here:

            outputPixelData = pixelData.AsParallel()
            .AsOrdered()
            .WithDegreeOfParallelism(numberOfThreads)
            .Select((element, index) =>ApplyKernelToPixel(pixelData, width, height, index % width, index / width, kernel))
            .ToArray();

            return outputPixelData;
        }

        // Chunked thread approach
        private static Rgba32[] Task4(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

            List<Thread> threads = new List<Thread>();
            int workPerProcess = pixelData.Length / numberOfThreads;

            for (int process = 0; process < numberOfThreads; process++)
            {
                // Calculate start and end indexes
                int startIndex = process * workPerProcess;
                int endIndex = (process == numberOfThreads - 1) ? pixelData.Length : startIndex + workPerProcess;

                // Assign work to the thread
                threads.Add(new Thread(() =>
                {
                    // Your code goes here:

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

            return outputPixelData;
        }

        // Chunked thread pool approach
        private static Rgba32[] Task5(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

            int workPerProcess = pixelData.Length / numberOfThreads;
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfThreads;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                for (int process = 0; process < numberOfThreads; process++)
                {
                    // Calculate start and end indexes
                    int startIndex = process * workPerProcess;
                    int endIndex = (process == numberOfThreads - 1) ? pixelData.Length : startIndex + workPerProcess;

                    // Assign work to the pool
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        // Your code goes here:

                        for (int index = startIndex; index < endIndex; index++)
                        {
                            int x = index % width;
                            int y = index / width;
                            outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                        }

                        if (Interlocked.Decrement(ref remaining) == 0) mre.Set();
                    });
                }

                // Wait for all threads to complete
                mre.WaitOne();
            }

            return outputPixelData;
        }

        // Chunked thread pool with dynamic partitioning
        private static Rgba32[] Task6(Rgba32[] pixelData, int width, int height, float[,] kernel, int numberOfThreads)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

            int workPerProcess = pixelData.Length / numberOfThreads;
            // Keep track of the number of threads remaining to complete
            int remaining = numberOfThreads;
            int nextIteration = 0;

            using (ManualResetEvent mre = new ManualResetEvent(false))
            {
                // Create each of the work items.
                for (int process = 0; process < numberOfThreads; process++)
                {
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        int index;

                        while ((index = Interlocked.Increment(ref nextIteration) - 1) < pixelData.Length)
                        {
                            // Your code goes here:

                            int x = index % width;
                            int y = index / width;
                            outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                        }

                        if (Interlocked.Decrement(ref remaining) == 0)
                            mre.Set();
                    });
                }

                // Wait for all threads to complete.
                mre.WaitOne();
            }

            return outputPixelData;
        }

        /// <summary>
        /// Sequential function which applies an input kernel to a single pixel
        /// </summary>
        /// <param name="pixelData">The input pixel data</param>
        /// <param name="width">Input image width</param>
        /// <param name="height">Input image height</param>
        /// <param name="pixelX">Current pixel x index</param>
        /// <param name="pixelY">Current pixel y index</param>
        /// <param name="kernel">The kernel</param>
        /// <returns>The new value for the current pixel</returns>
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
