﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MLAPI.Connection;

public class LobbyManager : NetworkedBehaviour
{
    // UI
    private LobbyCanvas canvas;

    // Keep track of connections on Server
    private int connected;
    private List<ulong> readyPlayers;
    private bool localReady;
    private GameInfo gameInfo;

    public override void NetworkStart()
    {
        // init vars
        readyPlayers = new List<ulong>();

        // Find the canvas.
        canvas = FindObjectOfType<LobbyCanvas>();
        canvas.initialize();

        // UI
        canvas.updatePlayerBarsOnServer(readyPlayers);
        canvas.initHostPanel(IsHost);

        // Destroy main menu music
        GameObject music = GameObject.FindGameObjectWithTag("MenuMusic");
        if (music != null)
        {
            Destroy(music);
        }

        if (IsServer)
        {
            // Call callbacks to track connections
            NetworkingManager.Singleton.OnClientConnectedCallback += ConnectionCallbackOnServer;
            NetworkingManager.Singleton.OnClientDisconnectCallback += ConnectionCallbackOnServer;

            // Create persistent GameInfo object to pass to GameManager
            gameInfo = new GameObject("GameInfo").AddComponent<GameInfo>();
            DontDestroyOnLoad(gameInfo.gameObject);

            connected = NetworkingManager.Singleton.ConnectedClientsList.Count;
            TellClientsToUpdateUI();
        }

        if (IsClient)
        {
            // Timeout after 5 minutes in lobby
            Invoke("OnDisconnectButton", 300);
        }
    }

    //--------------------------------------------
    // Button Callbacks
    //--------------------------------------------

    // Ready Button Clicked
    public void OnReadyButton()
    {
        // Toggle readiness
        localReady = !localReady;

        canvas.readyButton.GetComponentInChildren<Text>().text = localReady ? "Cancel" : "Ready";

        InvokeServerRpc(NotifyReadyForServer, localReady, NetworkingManager.Singleton.LocalClientId);
    }

    // Bot Button Clicked
    public void OnBotButton()
    {
        InvokeServerRpc(StartBotModeOnServer);
    }

    // Disconnect Button Clicked
    [ClientRPC]
    public void OnDisconnectButton()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryoneExcept(OnDisconnectButton, OwnerClientId);
            canvas.disableUI();
            Destroy(gameInfo.gameObject);
            StartCoroutine(DisconnectWithDelay());
        } 
        else
        {
            DisconnectOnClient();
        }
    }

    public void OnLevelLeftButton()
    {
        int numLevels = canvas.levelDropdown.options.Count;
        int newValue = canvas.levelDropdown.value - 1;
        if (newValue < 0)
        {
            newValue = numLevels - 1;
        }
        canvas.levelDropdown.value = newValue;
    }

    public void OnLevelRightButton()
    {
        int numLevels = canvas.levelDropdown.options.Count;
        int newValue = canvas.levelDropdown.value + 1;
        if (newValue >= numLevels)
        {
            newValue = 0;
        }
        canvas.levelDropdown.value = newValue;
    }

    //--------------------------------------------
    // Connected and Ready
    //--------------------------------------------

    [ServerRPC(RequireOwnership = false)]
    public void NotifyReadyForServer(bool isReady, ulong clientId)
    {
        if (isReady)
        {
            readyPlayers.Add(clientId);
        } else
        {
            readyPlayers.Remove(clientId);
        }

        TellClientsToUpdateUI();

        if (readyPlayers.Count >= 2)
        {
            // Enough players are ready, launch with delay
            startGame();
        }
    }

    // Server-Side, called by connect/disconnect callbacks
    public void ConnectionCallbackOnServer(ulong clientId)
    {
        connected = NetworkingManager.Singleton.ConnectedClientsList.Count;

        // Reset readiness
        if (readyPlayers.Contains(clientId))
        {
            readyPlayers.Remove(clientId);
        }

        TellClientsToUpdateUI();
    }

    private void TellClientsToUpdateUI()
    {
        InvokeClientRpcOnEveryone(SetDropdownValueOnClient, canvas.levelDropdown.value, canvas.winConDropdown.value);
        canvas.updatePlayerBarsOnServer(readyPlayers);
        InvokeClientRpcOnEveryone(UpdateConnectedOnClient, connected);
        InvokeClientRpcOnEveryone(UpdatePlayerNamesOnClient, canvas.playerBars.getLeftPlayerName(), canvas.playerBars.getRightPlayerName());
        InvokeClientRpcOnEveryone(UpdatePlayerScoresOnClient, canvas.playerBars.getLeftPlayerScore(), canvas.playerBars.getRightPlayerScore());
        InvokeClientRpcOnEveryone(UpdatePlayerReadyOnClient, canvas.playerBars.getLeftPlayerReady(), canvas.playerBars.getRightPlayerReady());
    }

    [ClientRPC]
    public void UpdateConnectedOnClient(int connected)
    {
        this.connected = connected;
        canvas.botButton.interactable = connected < 2; // Bot button disabled if there are two or more players
    }

    [ClientRPC]
    public void UpdatePlayerNamesOnClient(string leftPlayerName, string rightPlayerName)
    {
        canvas.playerBars.setPlayerNames(leftPlayerName, rightPlayerName);
    }

    [ClientRPC]
    public void UpdatePlayerScoresOnClient(string leftPlayerScore, string rightPlayerScore)
    {
        canvas.playerBars.setPlayerScores(leftPlayerScore, rightPlayerScore);
    }

    [ClientRPC]
    public void UpdatePlayerReadyOnClient(bool leftPlayerReady, bool rightPlayerReady)
    {
        canvas.playerBars.setReady(leftPlayerReady, rightPlayerReady);
    }

    // A client wants to play with bot
    [ServerRPC(RequireOwnership = false)]
    public void StartBotModeOnServer()
    {
        // Add bot flag to game info
        gameInfo.useBot = true;

        startGame();
    }

    // Server-Side, notifies clients and queues scene switch
    private void startGame()
    {
        int delay = 5;
        InvokeClientRpcOnEveryone(StartCountdownOnClient, delay);
        Invoke("SwitchScene", delay);

        gameInfo.winCon = (GameInfo.WinCondition)canvas.winConDropdown.value;
    }

    //--------------------------------------------
    // Disconnecting
    //--------------------------------------------

    private IEnumerator DisconnectWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        DisconnectOnClient();
    }

    private void DisconnectOnClient()
    {
        // Disconnect from network
        if (NetworkingManager.Singleton.IsConnectedClient)
        {
            NetworkingManager.Singleton.StopClient();
        }

        // Destroy old network
        Destroy(GameObject.FindGameObjectWithTag("Network"));

        // Go back
        SceneManager.LoadScene("Connection");
    }

    private void OnApplicationQuit()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(OnDisconnectButton);
        }
    }

    //--------------------------------------------
    // Dropdown
    //--------------------------------------------

    // Client tells server the dropdown value and server sends it to all clients
    public void OnDropdownChanged()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(SetDropdownValueOnClient, canvas.levelDropdown.value, canvas.winConDropdown.value);
        }
        else
        {
            InvokeServerRpc(SendDropdownValueToServer, canvas.levelDropdown.value, canvas.winConDropdown.value);
        }
    }

    // Client recieves new dropdown value from the server
    [ClientRPC]
    public void SetDropdownValueOnClient(int levelDropdownValue, int winConDropdownValue)
    {
        canvas.levelDropdown.SetValueWithoutNotify(levelDropdownValue);
        canvas.winConDropdown.SetValueWithoutNotify(winConDropdownValue);

        canvas.updateLevelPreview();
    }

    // Server recieves new dropdown value from a client
    [ServerRPC(RequireOwnership = false)]
    public void SendDropdownValueToServer(int levelDropdownValue, int winConDropdownValue)
    {
        canvas.levelDropdown.value = levelDropdownValue;
        canvas.winConDropdown.value = winConDropdownValue;
    }

    //--------------------------------------------
    // Countdown and Scene Switch
    //--------------------------------------------

    // Initiates scene switch and countdown (client side)
    [ClientRPC]
    public void StartCountdownOnClient(int delay)
    {
        canvas.startCountdown(delay);
    }

    // Server switches the Scene based on Dropdown selection
    public void SwitchScene()
    {
        // Remove callbacks
        NetworkingManager.Singleton.OnClientConnectedCallback -= ConnectionCallbackOnServer;
        NetworkingManager.Singleton.OnClientDisconnectCallback -= ConnectionCallbackOnServer;

        NetworkSceneManager.SwitchScene(canvas.levelDropdown.captionText.text);
    }
}
