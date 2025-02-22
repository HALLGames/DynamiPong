﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine.SceneManagement;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;

public class GameManagerBehaviour : NetworkedBehaviour
{
    // Canvas
    protected LevelCanvasBehaviour canvas;

    // Network
    protected Network network;

    // Bot
    protected bool useBot;

    // Score
    protected int leftScore;
    protected int rightScore;
    protected GameInfo.WinCondition winCon = GameInfo.WinCondition.Freeplay;
    protected int winScore = int.MaxValue;

    // Paddle & Ball
    protected PaddleBehaviour paddlePrefab;
    protected BallBehaviour ballPrefab;
    protected BallBehaviour ball;
    protected PaddleBehaviour leftPaddle;
    protected PaddleBehaviour rightPaddle;

    // Player names
    [SyncedVar]
    protected string leftName;
    [SyncedVar]
    protected string rightName;

    // Sound
    protected AudioSource wallHit;
    protected AudioSource paddleHit;
    protected AudioSource goalHit;

    // End of Game
    public enum PaddleWinState { LeftWon, RightWon, Tie}
    public enum GameWinState { Win, Loss, Tie}
    protected int playersReady = 0;

    //-----------------------------------------------
    // Initialization
    //-----------------------------------------------

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        initBackgroundMusic();
    }

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Update() in the method "new void Update()"
    /// </summary>
    protected void Update()
    {

    }

    /// <summary>
    /// Does NOT get called by Unity.
    /// Call this this method with base.NetworkStart() in "public override void NetworkStart()" 
    /// </summary>
    public override void NetworkStart()
    {
        // Find Canvas & Network
        canvas = FindObjectOfType<LevelCanvasBehaviour>();
        network = FindObjectOfType<Network>();

        // Add concede button listener
        canvas.concedeButton.onClick.AddListener(OnConcedeButton);

        // Sound
        initSound();

        if (IsServer)
        {
            // On Disconnect callback - end the game if a player disconnects
            NetworkingManager.Singleton.OnClientDisconnectCallback += clientDisconnectCallback;

            // Load in GameInfo created by lobby
            GameInfo gameInfo = FindObjectOfType<GameInfo>();
            if (gameInfo != null)
            {
                useBot = gameInfo.useBot;
                winCon = gameInfo.winCon;

                Destroy(gameInfo);
            }

            initWinCon();

            getPrefabs();

            // Spawn Paddles & Ball
            spawnPaddles();
            spawnBall();

            // Init scores
            leftScore = 0;
            rightScore = 0;
            InvokeClientRpcOnEveryone(InitPlayerScores);
        }
    }

    /// <summary>
    /// Override this method for custom background music
    /// </summary>
    protected virtual void initBackgroundMusic()
    {
        // Init Background Music
        GameObject backgroundMusicObject = new GameObject("BackgroundMusic");
        AudioSource backgroundMusic = backgroundMusicObject.AddComponent<AudioSource>();
        backgroundMusic.clip = Resources.Load<AudioClip>("Sound/Music/BGM1");
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }

    /// <summary>
    /// Override this method to customize what sounds are played
    /// </summary>
    protected virtual void initSound()
    {
        // initializes the wall hit and paddle hit sound effects
        wallHit = gameObject.AddComponent<AudioSource>();
        wallHit.clip = Resources.Load<AudioClip>("Sound/Common/WallHit");

        paddleHit = gameObject.AddComponent<AudioSource>();
        paddleHit.clip = Resources.Load<AudioClip>("Sound/Common/PaddleHit");

        // initializes the goal hit and paddle hit sound effects
        goalHit = gameObject.AddComponent<AudioSource>();
        goalHit.clip = Resources.Load<AudioClip>("Sound/Common/GoalHit");
        goalHit.volume = 0.5f;
    }

    protected void initWinCon()
    {
        switch(winCon)
        {
            case GameInfo.WinCondition.FirstTo10:
                winScore = 10;
                break;
            case GameInfo.WinCondition.FirstTo20:
                winScore = 20;
                break;
            case GameInfo.WinCondition.MostAfter5:
                canvas.timer.start(5 * 60);
                canvas.timer.TimerFinishedCallback += TimeOutScore;
                break;
            case GameInfo.WinCondition.MostAfter10:
                canvas.timer.start(5 * 60);
                canvas.timer.TimerFinishedCallback += TimeOutScore;
                break;
        }
        InvokeClientRpcOnEveryone(InitWinConOnClient, winCon);
    }

    [ClientRPC]
    public void InitWinConOnClient(GameInfo.WinCondition winCon)
    {
        canvas.initWinConText(winCon);
    }

    [ClientRPC]
    public void InitPlayerScores()
    {
        canvas.playerScores.initialize(leftName, rightName);
    }

    //-----------------------------------------------
    // Spawning
    //-----------------------------------------------

    // Override this method to change which prefabs are spawned
    protected virtual void getPrefabs()
    {
        paddlePrefab = Network.GetPrefab<PaddleBehaviour>("BasePaddle");
        ballPrefab = Network.GetPrefab<BallBehaviour>("BaseBall");
    }

    /// <summary>
    /// Override this method if you want to customize paddle spawning
    /// </summary>
    protected virtual void spawnPaddles()
    {
        // Paddle 1 - Positioned on the left

        ulong clientId = NetworkingManager.Singleton.ConnectedClientsList[0].ClientId;
        leftPaddle = Instantiate(paddlePrefab, Vector3.zero, Quaternion.identity);
        leftPaddle.GetComponent<NetworkedObject>().SpawnWithOwnership(clientId);
        leftPaddle.init(true);
        leftName = network.getConnectedPlayerNames()[clientId];

        if (useBot)
        {
            // Bot - Positioned on the right
            rightPaddle = Instantiate(paddlePrefab);
            rightPaddle.GetComponent<NetworkedObject>().Spawn();
            rightPaddle.setupBot();
            rightPaddle.init(false);
            rightName = "Bot";
        }
        else
        {
            // Paddle 2 - Positioned on the right
            clientId = NetworkingManager.Singleton.ConnectedClientsList[1].ClientId;
            rightPaddle = Instantiate(paddlePrefab, Vector3.zero, Quaternion.identity);
            rightPaddle.GetComponent<NetworkedObject>().SpawnWithOwnership(clientId);
            rightPaddle.init(false);
            rightName = network.getConnectedPlayerNames()[clientId];
        }
    }

    /// <summary>
    /// Override this method if you want to customize ball spawning
    /// </summary>
    protected void spawnBall()
    {
        // Spawn ball in center of screen
        ball = Instantiate(ballPrefab, transform.position, ballPrefab.transform.rotation);
        ball.GetComponent<NetworkedObject>().Spawn();
    }

   /// <summary>
   /// Override this method to customize the respawning of balls
   /// </summary>
    public virtual void respawnBall()
    {
        // If there is a ball, destroy it and create a new one
        if (ball != null)
        {
            ball.GetComponent<NetworkedObject>().UnSpawn();
            Destroy(ball.gameObject);
            spawnBall();
        }
    }

    //-----------------------------------------------
    // Sound
    //-----------------------------------------------

    public void PlaySound(string tag)
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(PlaySoundOnClient, tag);
        }
    }

    [ClientRPC]
    public void PlaySoundOnClient(string tag)
    {
        if (tag == "Paddle")
        {
            if (paddleHit.enabled)
            {
                paddleHit.Play();
            }
        }
        else if (tag == "Wall")
        {
            if (wallHit.enabled)
            {
                wallHit.Play();
            }
        }
        else if (tag == "Goal")
        {
            if (goalHit.enabled)
            {
                goalHit.Play();
            }
        }
    }

    //-----------------------------------------------
    // Scoring
    //-----------------------------------------------

    // Called by the goals.
    public void scoreGoal(bool ballHitOnLeft)
    {
        if (IsServer)
        {
            changeScore(ballHitOnLeft);

            InvokeClientRpcOnEveryone(UpdateScoreText, leftScore, rightScore);

            // If there is a ball, destroy it and create a new one
            if (ball != null && ball.GetComponent<NetworkedObject>().IsSpawned)
            {
                ball.GetComponent<NetworkedObject>().UnSpawn();
                Destroy(ball.gameObject);
                spawnBall();
            }
        }
    }

    [ClientRPC]
    public virtual void UpdateScoreText(int leftScore, int rightScore)
    {
        canvas.playerScores.updateScore(leftScore, rightScore);
    }

    /// <summary>
    /// Adds one point to the player opposite to the goal.
    /// Override this method to customize the scoring system.
    /// </summary>
    /// <param name="ballHitOnLeft"></param>
    public virtual void changeScore(bool ballHitOnLeft)
    {
        if (ballHitOnLeft)
        {
            rightScore++;
        }
        else
        {
            leftScore++;
        }

        checkWinScore();
    }

    // Check if win score has been reached
    protected void checkWinScore()
    {
        if (rightPaddle.IsBot())
        {
            if (leftScore >= winScore)
            {
                WinVsBotOnServer(PaddleWinState.LeftWon);
            }
            else if (rightScore >= winScore)
            {
                WinVsBotOnServer(PaddleWinState.RightWon);
            }
            
        } 
        else
        {
            if (leftScore >= winScore)
            {
                WinGameOnServer(PaddleWinState.LeftWon);
            }
            else if (rightScore >= winScore)
            {
                WinGameOnServer(PaddleWinState.RightWon);
            }
        }
    }

    // Determine who wins after time is up
    protected void TimeOutScore()
    {
        if (rightPaddle.IsBot())
        {
            if (leftScore > rightScore)
            {
                WinVsBotOnServer(PaddleWinState.LeftWon);
            }
            else if (leftScore < rightScore)
            {
                WinVsBotOnServer(PaddleWinState.RightWon);
            } else 
            {
                WinVsBotOnServer(PaddleWinState.Tie);
            }
        }
        else
        {
            if (leftScore > rightScore)
            {
                WinGameOnServer(PaddleWinState.LeftWon);
            }
            else if (leftScore < rightScore)
            {
                WinGameOnServer(PaddleWinState.RightWon);
            }
            else
            {
                WinGameOnServer(PaddleWinState.Tie);
            }
        }
    }

    //-----------------------------------------------
    // End of Game
    //-----------------------------------------------

    private void OnApplicationQuit()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(OnDisconnectButton);
        }
    }

    [ClientRPC]
    public void OnDisconnectButton()
    {
        if (IsHost)
        {
            // Disconnect everyone else, return to lobby
            InvokeClientRpcOnEveryoneExcept(OnDisconnectButton, OwnerClientId);
        }

        if (NetworkingManager.Singleton.IsConnectedClient)
        {
            NetworkingManager.Singleton.StopClient();
        }

        // Destroy old network
        Destroy(GameObject.FindGameObjectWithTag("Network"));

        // Go back
        SceneManager.LoadScene("Connection");
    }

    // Client clicked concede button
    public void OnConcedeButton()
    {
        InvokeServerRpc(ConcedeGameOnServer, NetworkingManager.Singleton.LocalClientId);
    }

    // Called when a client disconnects - we end the game
    protected void clientDisconnectCallback(ulong clientId)
    {
        endGame();
    }

    // Checks who clicked the concede button and assigns wins accordingly
    [ServerRPC (RequireOwnership = false)]
    public void ConcedeGameOnServer(ulong clientId)
    {
        PaddleWinState winState;
        if (leftPaddle.OwnerClientId == clientId)
        {
            winState = PaddleWinState.RightWon;
        } else
        {
            winState = PaddleWinState.LeftWon;
        }

        if (rightPaddle.IsBot())
        {
            WinVsBotOnServer(winState);
        } 
        else
        {
            WinGameOnServer(winState);
        }
    }

    // Server-side: Game was won between two clients (no bot)
    // Find the client that won and tell everyone
    protected void WinGameOnServer(PaddleWinState winState)
    {
        unspawnObjects();

        ulong winnerClientId = 0;
        bool hasWinner = true;
        switch (winState)
        {
            case PaddleWinState.LeftWon:
                winnerClientId = leftPaddle.OwnerClientId;
                break;
            case PaddleWinState.RightWon:
                winnerClientId = rightPaddle.OwnerClientId;
                break;
            case PaddleWinState.Tie:
                hasWinner = false;
                break;
        }

        // Update connectedPlayerScores on network
        if (hasWinner)
        {
            network.connectedPlayerScores[winnerClientId]++;
        }
        
        InvokeClientRpcOnEveryone(WinGameOnClient, hasWinner, winnerClientId);
    }

    // See if we are the client that won, then display the victory panel accordingly
    // If winnerClientId is null, then the game is a tie
    [ClientRPC]
    public void WinGameOnClient(bool hasWinner, ulong winnerClientId)
    {
        if (!hasWinner)
        {
            canvas.showVictoryPanel(GameWinState.Tie);
        }
        else if (winnerClientId == NetworkingManager.Singleton.LocalClientId)
        {
            canvas.showVictoryPanel(GameWinState.Win);
        }
        else
        {
            canvas.showVictoryPanel(GameWinState.Loss);
        }
    }

    // Server-side: game was won against a bot (one player, one bot)
    protected void WinVsBotOnServer(PaddleWinState winState)
    {
        unspawnObjects();

        InvokeClientRpcOnEveryone(WinVsBotOnClient, winState);
    }

    // Display panel based on win state
    [ClientRPC]
    public void WinVsBotOnClient(PaddleWinState winState)
    {
        switch(winState)
        {
            case PaddleWinState.LeftWon:
                canvas.showVictoryPanel(GameWinState.Win);
                break;
            case PaddleWinState.RightWon:
                canvas.showVictoryPanel(GameWinState.Loss);
                break;
            case PaddleWinState.Tie:
                canvas.showVictoryPanel(GameWinState.Tie);
                break;
        }
    }

    // Call by clients clicking "Continue" on victory panel
    // Wait for them before switching back to the lobby
    [ServerRPC (RequireOwnership = false)]
    public void WaitForClientsBeforeEnd()
    {
        playersReady++;
        if (playersReady >= 2 || rightPaddle.IsBot())
        {
            endGame();
        }
    }

    /// <summary>
    /// Override this method to specify objects to unspawn and destroy on victory.
    /// </summary>
    protected virtual void unspawnObjects()
    {
        // Unspawns the ball
        ball.GetComponent<NetworkedObject>().UnSpawn();
        Destroy(ball.gameObject);
    }

    /// <summary>
    /// Override this function to customize what happens when the game quits.
    /// Make sure to unspawn network objects.
    /// </summary>
    /// <param name="clientId"></param>
    protected virtual void endGame()
    {
        // Unspawn paddles and other things
        leftPaddle.GetComponent<NetworkedObject>().UnSpawn();
        Destroy(leftPaddle.gameObject);
        rightPaddle.GetComponent<NetworkedObject>().UnSpawn();
        Destroy(rightPaddle.gameObject);

        // Remove callbacks
        NetworkingManager.Singleton.OnClientDisconnectCallback -= clientDisconnectCallback;

        NetworkSceneManager.SwitchScene("Lobby");
    }
}
