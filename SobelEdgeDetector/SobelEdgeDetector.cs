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

        public static Image<Rgba32> PerformEdgeProcessing(in Image<Rgba32> image)
        {
            // Copy raw pixel data into an array
            Rgba32[] pixelData = new Rgba32[image.Width * image.Height];
            image.CopyPixelDataTo(pixelData);

            // Perform noise reduction
            Rgba32[] blurredPixelData = Convolve(pixelData, image.Width, image.Height, GaussianKernel5x5);

            // Convert to greyscale
            Rgba32[] greyscalePixelData = new Rgba32[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                // Calculate greyscale value using weighted luminosity method
                float average = ((0.3f * blurredPixelData[i].R) + (0.59f * blurredPixelData[i].G) + (0.11f * blurredPixelData[i].B)) / 256.0f;
                greyscalePixelData[i] = new Rgba32(average, average, average);
            }

            // Gradient calculation
            Rgba32[] sobelPixelDataHorizontal = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelHorizontal5x5);
            Rgba32[] sobelPixelDataVertical = Convolve(greyscalePixelData, image.Width, image.Height, SobelKernelVertical5x5);

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

            // Return the processed image
            return Image<Rgba32>.LoadPixelData(sobelMagnitude, image.Width, image.Height);
        }

        private static Rgba32[] Convolve(Rgba32[] pixelData, int width, int height, float[,] kernel)
        {
            Rgba32[] outputPixelData = new Rgba32[width * height];

#if PARALLEL_FOR

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;
                    outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                }
            });

#else

            

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width) + x;
                    outputPixelData[index] = ApplyKernelToPixel(pixelData, width, height, x, y, kernel);
                }
            }

#endif

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
                    // If the sample point is outisde the image, just sample the closest pixel along the edge
                    int sampleY = Math.Clamp(pixelY + kernelY - (kernel.GetLength(0) / 2), 0, height - 1);
                    int sampleX = Math.Clamp(pixelX + kernelX - (kernel.GetLength(1) / 2), 0, width - 1);

                    Rgba32 samplePixel = pixelData[(sampleY * width) + sampleX];
                    // Calculate the new values using the weightings in the kernel
                    sumR += samplePixel.R * kernel[kernelY, kernelX];
                    sumG += samplePixel.G * kernel[kernelY, kernelX];
                    sumB += samplePixel.B * kernel[kernelY, kernelX];
                }
            }

            return new Rgba32(sumR / 256.0f, sumG / 256.0f, sumB / 256.0f);
        }

    }
}
