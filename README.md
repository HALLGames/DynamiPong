![DynamiPong](https://github.com/HALLGames/DynamiPong/blob/master/Images/Title.png)
DynamiPong is a take on the classic video game [Pong](https://en.wikipedia.org/wiki/Pong), using the Unity engine. Play on a variety or gamemodes against a bot or against your friends.

[**Download**](https://github.com/HALLGames/DynamiPong/blob/master/Installer/DynamiPong-0.1.exe)

## Screenshots

![Bots](https://github.com/HALLGames/DynamiPong/blob/master/Images/BotExample.png)
*Play against bots for a singleplayer experience.*

![Multiplayer](https://github.com/HALLGames/DynamiPong/blob/master/Images/MultiplayerGamemodes1.PNG)
*Duel your friends.*

![Gamemodes](https://github.com/HALLGames/DynamiPong/blob/master/Images/MultiplayerGamemodes2.PNG)
*Explore alternate gamemodes.*

## Development Environment

Install the following tools to setup the development environment:

- Discord - https://discord.com/
  - [Join our Discord](https://discord.gg/nSVjNRs)

- Visual Studio 2019 Community: https://visualstudio.microsoft.com/vs/community/
  - see also https://docs.microsoft.com/en-us/visualstudio/cross-platform/visual-studio-tools-for-unity?view=vs-2019

- Unity 2019.4.3f1 - https://unity3d.com/get-unity/download

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
- Duplicate the scene "TemplateLevel" (in Assets/Scenes/Levels) and rename it with your level name (Duplicate Shortcut: ctrl-d).

### Preparing the Scripts
- In Assets/Scripts, duplicate the Template folder and rename it with your level name.
  - Unity should give you an error stating that cs files are conflicting. This is expected and will be resolved with renaming.
- Rename all files with the following name scheme: LevelNameBall, LevelNamePaddle, etc.
  - Ex. A level called Level2 will have: Level2Ball, Level2Paddle, Level2Goal, etc.
- Open each script in Visual Studio. 
- Change each file's Class name to their file name.
  - DO NOT use VS's rename command. It will also rename the actual template scripts and make a big mess.
  - Ex. Change ```public Class TemplatePaddle : PaddleBehaviour``` to ```public Class LevelNamePaddle : PaddleBehaviour``` 
- In your level scene, go through every game object and replace the scripts with the new ones that you made.
- Reattach the UI fields for the LevelCanvas, which found under the Canvas game object.
- Open the Prefabs folder (Assets/Prefabs) and duplicate the TemplateLevel folder.
  - Rename the prefabs with the same name scheme
  - Change the Prefab Hash Generator in the NetworkObject component to their name
  - Replace the script with your new one
  
### Running your new level
- In build settings, add the Connection and Lobby scenes if they are not already there, then add your level scene to the "Scenes to Build" list.
- In the Connection scene, open the game object named "Network", then open the NetworkingManager component.
  - Add any prefabs you added to the NetworkedPrefabs list, then add the name of your scene to the list of Registered Scene Names.
- In the Lobby scene, click on the LevelDropdown game object, which is a child of the Canvas.
  - In the Dropdown component, add your level name as one of the options. The spelling here must be exact.
- Your scene should now be able to run with the base behaviours.
  
  ### Add new things
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
  - You can look at any of the Behaviour classes to see what methods can be overridden. Only virtual, abstract and override methods can be overridden.
  - If you really need to, you can alter the Behaviour classes and add virtual methods in it. However, be careful, since everyone uses them.
- Any new Scripts, Sprites, Materials, Resources, etc. should be put in a new folder with your level name under the respective folder in Assets.
  - Ex. Sprites for a level called "Level2" should be put in the folder Assets/Sprites/Level2
  - If you want to have slightly altered common assets, copy them to the new folder.
- When you copy prefabs with NetworkObjects, remember to change the Prefab Hash Generator, otherwise MLAPI gets mad.
- One of the most important methods to override is getPrefabs() in the GameManager, since it specifies which prefabs are used. 
  - This is already done in the Template, but you still need to rename the classes and strings.
  

## "Planned" Features (no guarantees)
- [Level Ideas](https://github.com/HALLGames/DynamiPong/blob/master/LevelIdeas.md)
