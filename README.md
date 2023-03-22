# F21SM Masterclass Lab Exercises


# Installation instructions

There are two ways to install C# to run the project:
* Download the dotnet development runtime (recommended)
* Download the Docker development environment which contains the dotnet runtime

You will also need an IDE installed, I recommend [VSCode](https://code.visualstudio.com/download).

## Option 1: Manual installation (recommended)

1. Download and install **version 7** of the [dotnet SDK](https://dotnet.microsoft.com/en-us/download)
   
   If you want to install the dotnet SDK using a package manager, make sure to install the `dotnet-sdk` version seven

2. Clone this repository to disk using `git clone https://github.com/SolomonBaarda/f21sm-lab.git` or download it as a zip
3. Open the `f21sm-lab` folder in your IDE of choice
    
    If you are using VSCode, select the *Extensions* tab on the left, search for C#, and install the Microsoft C# extension

    If a dialog box appears in the bottom right saying *"Required assets to build and debug are missing from 'f21sm-lab'. Add them?"* Select **YES**

4. Navigate to the `SobelEdgeDetector` directory

5. Run `dotnet restore` to download all package dependencies 


## Option 2: Docker installation

I have provided installation instructions for Docker, if you wish to use it. **THIS IS OPTIONAL**

1. Install [Docker Desktop](https://docs.docker.com/get-docker/) (**or any other version of Docker if you know how to use it**)

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

1. Create a terminal window in your IDE of choice 
2. Navigate to the `SobelEdgeDetector` directory
3. Run the program using `dotnet run <task number> <input file path> <output file path> <number of cores>` 
   
   e.g. To run task 1 on a four core machine, use: `dotnet run 1 images/sunset.jpg images/output.png 4`

   Check that the image located at `images/output.png` has been generated successfully 
