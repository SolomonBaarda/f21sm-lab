# Installation instructions
1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Install [VSCode](https://code.visualstudio.com/download)
3. From Docker Desktop, select *Dev Environments* and press *Create new* and *Get started*
5. Add the git repo `https://github.com/SolomonBaarda/f21sm-lab.git`
6. Ensure that the VSCode IDE box is selected
7. Press continue, then continue, then open in VSCode
8. In VSCode, select the *Extensions* tab on the left, search for C#, and install the Microsoft C# extension in the f21sm-lab container
9. If a dialog box appears in the bottom right saying *"Required assets to build and debug are missing from 'f21sm-lab'. Add them?"* Select **YES**
10. If this fails, press `ctrl + shift p` and select *".NET Restore Project"*

# Running the program
1. Create a terminal window in VSCode from the navigation bar at the top of the screen
2. Navigate to the `SobelEdgeDetector` directory
3. Run the project using `dotenv run <cores> <input file path> <output file path>` e.g. `dotenv run 4 images/sunset.jpg images/output.png`
