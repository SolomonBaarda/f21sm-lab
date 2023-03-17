# Sobel Edge Detection algorithm
Image processing algorithm used for detecting the edges of shapes within an image
Image processing algorithms are generally delightfully parallel – normally for each stage in the algorithm, each pixel can be processed independently 
The convolve method is the core function of this example – it takes a kernel of multiplier values, and for each pixel, it multiplies the nearby pixels by the kernel, sums the values, and sets the output pixel to that sum


# Installation instructions
1. Install [Docker Desktop](https://docs.docker.com/get-docker/) (or any other verion of Docker if you know how to use it)

    If you are on Windows, you will need to enable virtualisation in your BIOS and update WSL (Windows Subsystem for Linux) using `wsl --update`

2. Install [VSCode](https://code.visualstudio.com/download) (or any other editor if you know how to use it with Docker)
3. From Docker Desktop, select *Dev Environments* and press *Create new*, then *Get started*
5. Add the git repo `https://github.com/SolomonBaarda/f21sm-lab.git`
6. Ensure that the VSCode IDE box is selected. If you get a prompt to install the Visual Studio Code Remote Containers Extension, follow the instructions provided (you'll need to press retry once you have it installed).
7. Press continue, then continue, then open in VSCode
8. In VSCode, select the *Extensions* tab on the left, search for C#, and install the Microsoft C# extension in the f21sm-lab container
9. If a dialog box appears in the bottom right saying *"Required assets to build and debug are missing from 'f21sm-lab'. Add them?"* Select **YES**
10. If this fails, then run `dotnet restore` from a command shell

# Running the program
1. Create a terminal window in VSCode from the navigation bar at the top of the screen
2. Navigate to the `SobelEdgeDetector` directory
3. Run the program using `dotnet run <task number> <input file path> <output file path>` 
   
   e.g. `dotnet run 1 images/sunset.jpg images/output.png`

# Exercises

## Task 1

1. Run the sequential implementation on some images of your choice. Some sample images have been included in the `SobelEdgeDetector/images` directory, though feel free to use other images if you would like
2. Choose an image that you would like to use for benchmarking performance 
3. Measure the runtime of the sequential implementation 

## Task 2

1. Implement lab task two using the `Parallel.For` construct.
2. Measure the runtime of your parallel solution and calculate the speedup compared to the sequential implementation

## Task 3

1. Implement lab task three using the `PLINQ Select` method, an equivalent of the `map` higher order function

    Note that you can use the syntax `(element, index) => { your code goes here }` within `Select` to get the index of the current iteration. Using the index, you can calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width`.

    The `PLINQ` documentation can be found [here](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/introduction-to-plinq). You might find `ToArray` and `AsOrdered` to be useful for your implementation.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of `PLINQ` compare to `Parallel.For`?

## Task 4

1. Implement lab task four using threads and a chunking approach. Skeleton code has been included.
   
   You will likely need to calculate the `x` and `y` position of the current pixel using `x = index % width` and `y = index / width`.

2. Measure the runtime of your new parallel solution and calculate the speedup compared to the sequential implementation. 

    How does the performance of chunking threads compare to `Parallel.For` and `PLINQ`?

## Task 5

1. 

## Task 6
1. 


