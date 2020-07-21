# DynamiPong
Multiplayer Pong, with some twists...

## Documentation
- [Best Practices](http://www.glenstevens.ca/unity3d-best-practices/)

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

## Creating a new level
- Create a new branch for your level
- Duplicate the scene "Level1" (in Assets/Scenes/Levels) and rename it with your level name (Duplicate Shortcut: ctrl-d).

### Preparing the Scripts
- In Assets/Scripts, create a new folder it with your level name.
- Create 5 new scripts for Ball, Paddle, GameManager, LevelCanvas and Goal
  - They should all follow the following name scheme: LevelNameBall, LevelNamePaddle, etc.
  - Ex. A level called Level2 will have: Level2Ball, Level2Paddle, Level2Goal, etc.
- Open each script in Visual Studio. Change each of them to extend their respective behaviour instead of MonoBehaviour.
  - Ex. In Level2Ball.cs, ```Level2Ball : BallBehaviour```. In Level2Paddle.cs, ```Level2Paddle : PaddleBehaviour``` 
- In your level scene, go through every game object and replace the scripts with the new ones that your made.
  - Your scene should now be able to run with the base behaviours.
- Whenever you want custom logic, open your script and override what you want to modify. 
  - If you want to maintain base behaviour, add base.MethodName() to your method.
  - The Update() and Start() methods cannot be overridden, so use the "new" keyword instead, then call the base.
    ```
    new void Update() 
    {
      base.Update();
      
      // Your logic here
    }
    ```
    
### Add new things
- Any new Scripts, Sprites, Materials, Resources, Prefabs, etc. should be put in a new folder with your level name under the respective folder in Assets.
  - Ex. Sprites for a level called "Level2" should be put in the folder Assets/Sprites/Level2
  - If you want to have slightly altered common assets, copy them to the new folder.
- When you copy prefabs with NetworkObjects, make sure you also change the Prefab Hash Generator in the NetworkObject component.
- In your GameManager, override getPrefabs() with your own prefab names.
  
### Running your new level
- In build settings, add the Connection and Lobby scenes if they are not already there, then add your level scene to the "Scenes to Build" list.
- In the Connection scene, open the game object named "Network", then open the NetworkingManager component.
  - Add any prefabs you added to the NetworkedPrefabs list, then add the name of your scene to the list of Registered Scene Names.
- In the Lobby scene, click on the LevelDropdown game object, which is a child of the Canvas.
  - In the Dropdown component, add your level name as one of the options. The spelling here must be exact.
