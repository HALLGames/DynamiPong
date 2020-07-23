using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MLAPI.Messaging;

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

    // Paddle & Ball
    protected PaddleBehaviour paddlePrefab;
    protected BallBehaviour ballPrefab;
    protected BallBehaviour ball;
    protected PaddleBehaviour leftPaddle;
    protected PaddleBehaviour rightPaddle;

    // Player names
    protected string leftName;
    protected string rightName;

    /// <summary>
    /// Does NOT get called by Unity
    /// Call this method with base.Start() in the method "new void Start()"
    /// </summary>
    protected void Start()
    {
        
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

        // Add disconnect button listener
        canvas.disconnectButton.onClick.AddListener(OnDisconnectButton);

        if (IsServer)
        {
            // On Disconnect callback - end the game if a player disconnects
            NetworkingManager.Singleton.OnClientDisconnectCallback += endGame;

            // Check if we should use a bot - bot flag created by lobby
            GameObject botFlag = GameObject.FindGameObjectWithTag("BotFlag");
            if (botFlag != null)
            {
                useBot = true;
                Destroy(botFlag);
            }

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

    // Override this method to change which prefabs are spawned
    protected virtual void getPrefabs()
    {
        paddlePrefab = Network.GetPrefab<PaddleBehaviour>("BasePaddle");
        ballPrefab = Network.GetPrefab<BallBehaviour>("BaseBall");
    }

    protected void spawnPaddles()
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

    // Spawn ball in center of screen
    protected void spawnBall()
    {
        ball = Instantiate(ballPrefab, transform.position, ballPrefab.transform.rotation);
        ball.GetComponent<NetworkedObject>().Spawn();
    }

    // Called by the goals.
    public void scoreGoal(bool ballHitOnLeft)
    {
        if (IsServer)
        {
            changeScore(ballHitOnLeft);

            InvokeClientRpcOnEveryone(UpdateScoreText, leftScore, rightScore);

            // If there is a ball, destroy it and create a new one
            if (ball != null)
            {
                ball.GetComponent<NetworkedObject>().UnSpawn();
                Destroy(ball.gameObject);
                spawnBall();
            }
        }
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
    }

    [ClientRPC]
    public virtual void UpdateScoreText(int leftScore, int rightScore)
    {
        canvas.leftScoreText.text = leftName + ": " + leftScore.ToString();
        canvas.rightScoreText.text = rightName + ": " + rightScore.ToString();
    }

    public void OnDisconnectButton()
    {
        // Disconnect ourselves
        NetworkingManager.Singleton.DisconnectClient(NetworkingManager.Singleton.LocalClientId);
        // Destroy old network
        Destroy(GameObject.FindGameObjectWithTag("Network"));
        // Go back
        SceneManager.LoadScene("Connection");
    }

    protected void endGame(ulong clientId)
    {
        ball.GetComponent<NetworkedObject>().UnSpawn();
        // TODO: Check if we need to unspawn paddles

        NetworkSceneManager.SwitchScene("Lobby");
    }
}
