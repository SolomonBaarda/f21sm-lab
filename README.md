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
3. Run the project using `dotnet run <cores> <input file path> <output file path>` e.g. `dotnet run 4 images/sunset.jpg images/output.png`


# Exercises

