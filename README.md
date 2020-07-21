# DynamiPong
Multiplayer Pong, with some twists...

## Documentation
TBD

## Development Environment

Install the following tools to setup the development environment:

- Discord - Forge Networking developers and questions: https://discord.com/
  - [Join our Discord](https://discord.gg/nSVjNRs)

- Visual Studio 2019 Community: https://visualstudio.microsoft.com/vs/community/
  - see also https://docs.microsoft.com/en-us/visualstudio/cross-platform/visual-studio-tools-for-unity?view=vs-2019

- Unity 2019.4.4f1 - https://unity3d.com/get-unity/download

- WinMerge - diffing: https://winmerge.org/

- Windows Terminal - https://aka.ms/terminal
  - Open command prompt, and type ```git``` for repo access and version control

- git Large File Storage (LFS) extension - https://git-lfs.github.com/
  - From commandline, run ```git lfs install``` to initialize
  
- MLAPI - https://mlapi.network/
  - Docs - https://mlapi.network/wiki/home/

## Project Setup
Use the command (in the folder you want):
```
git clone https://github.com/HALLGames/DynamiPong
```
Open Unity Hub, click "Add", then select the folder "DynamiPong/DynamiPong". 

### Creating a new level
- Create a new branch for your level
- Duplicate the scene "Level1" (in Assets/Scenes) and rename it with your level name.
- Duplicate the folder Level1 in Assets/Scripts for your scripts and rename it with your level name.
- Relink all scripts in your project to the new scripts.
- Any new Sprites, Materials, Resources, Prefabs, etc. should be put in a new folder with your level name under the respective folder in Assets.
  - ex. Sprites for a level called "Level2" should be put in the folder Assets/Sprites/Level2
- In build settings, add the Connection and Lobby scenes if they are not already there, then add your level scene to the "Scenes to Build" list.
- In the Connection scene, open the game object named "Network", then open the NetworkingManager component.
  - Add any prefabs you added to the NetworkedPrefabs list, then add the name of your scene to the list of Registered Scene Names.
- In the Lobby scene, click on the LevelDropdown game object, which is a child of the Canvas.
  - In the Dropdown component, add your level name as one of the options. The spelling here must be exact.
