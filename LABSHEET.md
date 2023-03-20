# Sobel Edge Detection algorithm

The Sobel Edge Detection algorithm is an image processing technique used for identifying the edges of objects within an image. Each pixel in the image can be processed independently, making parallelising this algorithm delightful. The purpose of this lab exercise is to give the students time to gain first hand experience in using the parallel techniques discussed in the lecture.

# Running the program

1. Create a terminal window in your IDE of choice 
2. Navigate to the `SobelEdgeDetector` directory
3. Run the program using `dotnet run <task number> <input file path> <output file path> <number of cores>` 
   
   e.g. To run task 1 on a four core machine, use: `dotnet run 1 images/sunset.jpg images/output.png 4`

# Exercises

Please refer to the examples in `CountingPrimes/Program.cs` for help with the C# syntax. These are the examples that we discussed in the lecture. 

## Task 1

1. Run the sequential implementation on some images of your choice. Some sample images have been included in the `SobelEdgeDetector/images` directory, though feel free to use other images if you would like
2. Choose an image that you would like to use for benchmarking performance 
3. Measure the runtime of the sequential implementation 

## Task 2

1. Implement lab task two within the `Convolve` method, using the `Parallel.For` construct.

    Pass in `new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }` as a parameter before the lambda function to ensure that the maximum degree of parallelism is set.

2. Measure the runtime of your parallel solution and calculate the speedup compared to the sequential implementation

## Task 3

1. Implement lab task three within the `Convolve` method, using the `PLINQ Select` method, an equivalent of the `map` higher order function

    Note that you can use the syntax `(element, index) => { your code goes here }` within `Select` to get the index of the current iteration. Using the index, you can calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width`.

    The `PLINQ` documentation can be found [here](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq). You might find `ToArray` and `AsOrdered` to be useful for your implementation.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of `PLINQ` compare to `Parallel.For`?

## Task 4

1. Implement lab task four within the `Convolve` method, using a thread pool and a chunking approach. Skeleton code has been included.
   
   You will likely need to calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width`.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of chunking threads compare to `Parallel.For` and `PLINQ`?

## Task 5

1. Implement lab task five within the `Convolve` method, using a thread pool and fully dynamic partitioning. Refer to the examples within the `CountingPrimes` project.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of the fully dynamic partitioning approach compare to the chunking approach? Do we see much difference in performance?

## Task 6 (optional)

1. Implement lab task six within the `PerformEdgeProcessing` method. You should modify the program so that it now outputs the original image, with the detected edges on top. You can optionally colour the edges to make them stand out. 

    Bonus points if you can implement this using a parallel technique. 
