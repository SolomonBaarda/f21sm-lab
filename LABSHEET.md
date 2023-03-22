# Sobel Edge Detection algorithm

The Sobel Edge Detection algorithm is an image processing technique used for identifying the edges of objects within an image. Each pixel in the image can be processed independently, making parallelising this algorithm delightful. The purpose of this lab exercise is to give the students time to gain first hand experience in using the parallel techniques discussed in the lecture.

# Running the program

1. Follow the installation instructions as described in [the project readme](./README.md)
2. Open a terminal window in your IDE of choice 
3. Navigate to the `SobelEdgeDetector` directory
4. Ensure that the project dependencies have been installed. Run `dotnet restore` to double check
5. Run the program using `dotnet run <task number> <input file path> <output file path> <number of cores>` 
   
   e.g. To run task 1 on a four core machine, use: `dotnet run 1 images/sunset.jpg images/output.png 4`

# Exercises

Please refer to the examples in [LectureExample/Program.cs](LectureExample/Program.cs) for help with the C# syntax. These are the examples that we discussed in the lecture. 

**For the following tasks, please record your answers in a document or spreadsheet.** We will go over the solutions as a class at the end of the session.

## Task 1

1. Run the sequential implementation on some images of your choice. Some sample images have been included in the `SobelEdgeDetector/images` directory, though feel free to use other images if you would like.
2. Choose an image that you would like to use for benchmarking performance.
3. Measure the runtime of the sequential implementation and record it in your document.

Now take some time to familiarize yourself with the code. 
* [Program.cs](./SobelEdgeDetector/Program.cs) - contains the main method, here we load the files from disk, check command line arguments, and run the main image processing function
* [SobelEdgeDetector.cs](./SobelEdgeDetector/SobelEdgeDetector.cs) - contains the main image processing functions
  
    Kernels - these are 2D arrays of weighting values. When performing a processing stage in image processing, we loop over the entire image and for each pixel, we sample all surrounding pixels which fall within this kernel. Each of those pixels is then multiplied by the kernel weighting value and summed. This value becomes the output for that pixel for the current stage.

    `ApplyKernelToPixel` - this function is used to calculate the output value for each pixel in the input image.

    `Convolve` - this function is responsible for applying a kernel to an entire image. It is the main function requiring parallelization.

    `PerformEdgeProcessing` - this is the main image processing function which performs several processing stages using the `Convolve` method.


## Task 2

1. Implement the function called `Task2`, using the `Parallel.For` construct.

    Pass in `new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads }` as a parameter before the lambda function to ensure that the maximum degree of parallelism is set.

    **Make sure you pass in the value `2` (for lab task 2) when running the program from the command line**.

2. Measure the runtime of your parallel solution and calculate the speedup compared to the sequential implementation. Record these values in your document. 

    Answer the following questions:
    * Did you see a significant performance increase? 
    * How much effort did it require to get a decent speedup?

## Task 3

1. Implement the function called `Task3`, using the PLINQ `Select` method, an equivalent of the `map` higher order function.

    Hint: you can use the syntax `(element, index) => { your code goes here }` within `Select` to get the index of the current iteration. Using the index, you can calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width`.

    The `PLINQ` documentation can be found [here](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq). You might find `ToArray` and `AsOrdered` to be useful for your implementation.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. Record these values in your document.

    Answer the following questions:
    * Did you see a significant performance increase? 
    * How much effort did it require to get a decent speedup?
    * How does the performance of `PLINQ` compare to `Parallel.For`?

## Task 4

1. Implement the function called `Task4`, using basic threads and a chunking approach. Skeleton code has been included.
   
   You will likely need to calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width` like before.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. Record these values in your document.

    Answer the following questions:
    * Did you see a performance increase using basic threads?
    * How does the performance of chunking threads compare to `Parallel.For` and `PLINQ`?
    * Considering that the `Convolve` method is called multiple times during processing, and each time we create new threads the destroy them, which parallel technique could we use to get better performance?

## Task 5

1. Implement the function called `Task5`, using a thread pool and a chunking approach. Skeleton code has been included.
   
   You will likely need to calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width` like before.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. Record these values in your document.

    Answer the following questions:
    * How does the performance of reusing thread objects compare to the basic thread approach?

## Task 6

1. Implement the function called `Task6`, using a thread pool and fully dynamic partitioning. Skeleton code has been included.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of the fully dynamic partitioning approach compare to the chunking approach? Record these values in your document.

    Answer the following questions:
    * Do we see much difference in performance?
    * Is the performance cost of fully dynamic partitioning worth it in this case?
    * How could we modify our dynamic partitioning approach to get better performance? *Hint: each thread needs to do enough work to justify starting it up*

## Task 7 (optional)

1. Implement lab task seven within the `PerformEdgeProcessing` method. You should modify the program so that it now outputs the original image, with the detected edges on top. You can optionally colour the edges to make them stand out. 

    Bonus points if you can implement this using a parallel technique of your choice. 

## Task 8 (optional)

1. Which other areas in the `PerformEdgeProcessing` could still be parallelized?
2. Choose a parallel approach of your choice and implement these changes