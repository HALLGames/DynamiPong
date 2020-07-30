using System.Collections;
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

            // Update Text
            InvokeClientRpcOnEveryone(UpdateScoreText, leftScore, rightScore);
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
        backgroundMusic.clip = Resources.Load<AudioClip>("Sound/Music/BackgroundMusic");
        backgroundMusic.loop = true;
        backgroundMusic.Play();
    }

    protected void initWinCon()
    {
        switch(winCon)
        {
            case GameInfo.WinCondition.FirstTo15:
                winScore = 15;
                break;
            case GameInfo.WinCondition.FirstTo30:
                winScore = 30;
                break;
            case GameInfo.WinCondition.MostAfter5:
                canvas.timer.startTimer(5 * 60);
                canvas.timer.TimerFinishedCallback += TimeOutScore;
                break;
            case GameInfo.WinCondition.MostAfter10:
                canvas.timer.startTimer(5 * 60);
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
        leftPaddle = Instantiate<PaddleBehaviour>(paddlePrefab, Vector3.zero, Quaternion.identity);
        leftPaddle.GetComponent<NetworkedObject>().SpawnWithOwnership(clientId);
        leftPaddle.init(true);
        leftName = network.getConnectedPlayerNames()[clientId];

        if (useBot)
        {
            // Bot
            rightPaddle = Instantiate(paddlePrefab);
            rightPaddle.setupBot();
            rightPaddle.init(false);
            rightName = "Bot";
        }
        else
        {
            // Paddle 2 - Positioned on the right
            clientId = NetworkingManager.Singleton.ConnectedClientsList[1].ClientId;
            rightPaddle = Instantiate<PaddleBehaviour>(paddlePrefab, Vector3.zero, Quaternion.identity);
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
        canvas.leftScoreText.text = leftName + ": " + leftScore.ToString();
        canvas.rightScoreText.text = rightName + ": " + rightScore.ToString();
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

    [ClientRPC]
    public void OnDisconnectButton()
    {
        if (IsHost)
        {
            // Disconnect everyone else, return to lobby
            InvokeClientRpcOnEveryoneExcept(OnDisconnectButton, OwnerClientId);
        }

        // Destroy old network
        Destroy(GameObject.FindGameObjectWithTag("Network"));
        // Go back
        SceneManager.LoadScene("Connection");
    }

    public void OnConcedeButton()
    {
        InvokeServerRpc(ConcedeGameOnServer, NetworkingManager.Singleton.LocalClientId);
    }

    // Called when a client disconnects - we end the game
    protected void clientDisconnectCallback(ulong clientId)
    {
        endGame();
    }

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

    protected void WinGameOnServer(PaddleWinState winState)
    {
        unspawnObjects();

        ulong? winnerClientId = null;
        switch (winState)
        {
            case PaddleWinState.LeftWon:
                winnerClientId = leftPaddle.OwnerClientId;
                break;
            case PaddleWinState.RightWon:
                winnerClientId = rightPaddle.OwnerClientId;
                break;
        }

        if (winnerClientId != null)
        {
            network.connectedPlayerScores[(ulong) winnerClientId]++;
        }
        
        InvokeClientRpcOnEveryone(WinGameOnClient, winnerClientId);
    }

    [ClientRPC]
    public void WinGameOnClient(ulong? winnerClientId)
    {
        if (winnerClientId == null)
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

    protected void WinVsBotOnServer(PaddleWinState winState)
    {
        unspawnObjects();

        InvokeClientRpcOnEveryone(WinVsBotOnClient, winState);
    }

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

    [ServerRPC (RequireOwnership = false)]
    public void WaitForClientsBeforeEnd()
    {
        playersReady++;
        if (playersReady >= 2 || rightPaddle.IsBot())
        {
            endGame();
        }
    }

    protected virtual void unspawnObjects()
    {
        ball.GetComponent<NetworkedObject>().UnSpawn();
        Destroy(ball.gameObject);
    }

    /// <summary>
    /// Override this function to customize what happens when the game ends.
    /// Make sure to unspawn network objects.
    /// </summary>
    /// <param name="clientId"></param>
    protected virtual void endGame()
    {
        // Unspawn paddles and other things
        if (IsHost)
        {
            leftPaddle.GetComponent<NetworkedObject>().UnSpawn();
        }

        NetworkSceneManager.SwitchScene("Lobby");
    }
}
