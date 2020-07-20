using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;

public class GameManager : NetworkedBehaviour
{
    // Bot
    public bool useBot;

    // Score
    public int leftScore;
    public int rightScore;
    public Text leftScoreText;
    public Text rightScoreText;

    // Ball
    private Ball ballPrefab;
    private Ball ball;

    public override void NetworkStart()
    {
        if (IsServer)
        {
            NetworkingManager.Singleton.OnClientDisconnectCallback += endGame;

            // Spawn paddles
            spawnPaddles();

            // Spawn Ball
            spawnBall();

            // Init scores
            leftScore = 0;
            rightScore = 0;
            InvokeClientRpcOnEveryone(UpdateScoreText, leftScore, rightScore);
        }
    }

    private void spawnPaddles()
    {
        // Get prefab
        Paddle paddlePrefab = Network.GetPrefab<Paddle>("Paddle");

        // Paddle 1 - Positioned on the left

        ulong clientId = NetworkingManager.Singleton.ConnectedClientsList[0].ClientId;
        Paddle paddle1 = Instantiate<Paddle>(paddlePrefab, Vector3.zero, Quaternion.identity);
        paddle1.GetComponent<NetworkedObject>().SpawnWithOwnership(clientId);
        paddle1.setSide(true);

        if (useBot)
        {
            // Bot
            Instantiate(paddlePrefab).setupBot();
        }
        else
        {
            // Paddle 2 - Positioned on the right
            clientId = NetworkingManager.Singleton.ConnectedClientsList[1].ClientId;
            Paddle paddle2 = Instantiate<Paddle>(paddlePrefab, Vector3.zero, Quaternion.identity);
            paddle2.GetComponent<NetworkedObject>().SpawnWithOwnership(clientId);
            paddle2.setSide(false);
        }
    }

    public void scoreGoal(bool onLeft)
    {
        // Adds one point to the player opposite to the goal
        if (IsServer)
        {
            if (onLeft)
            {
                rightScore++;
            }
            else
            {
                leftScore++;
            }

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

    [ClientRPC]
    public void UpdateScoreText(int leftScore, int rightScore)
    {
        leftScoreText.text = leftScore.ToString();
        rightScoreText.text = rightScore.ToString();
    }

    // Spawn ball in center of screen
    void spawnBall()
    {
        // If we don't have the prefab, find it
        if (ballPrefab == null)
        {
            ballPrefab = Network.GetPrefab<Ball>("Ball");
        }

        ball = Instantiate(ballPrefab, transform.position, ballPrefab.transform.rotation);
        ball.GetComponent<NetworkedObject>().Spawn();
    }

    void endGame(ulong clientId)
    {
        ball.GetComponent<NetworkedObject>().UnSpawn();
        // TODO: Check if we need to unspawn paddles

        NetworkSceneManager.SwitchScene("Lobby");
    }
}
