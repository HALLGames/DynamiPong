using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkedBehaviour
{
    // UI
    private LobbyCanvas canvas;

    // Keep track of connections on Server
    private int connected;
    private List<ulong> readyPlayers;
    private bool localReady;

    public override void NetworkStart()
    {
        // init vars
        readyPlayers = new List<ulong>();

        // Find the canvas. Initialize because it gets updated before Start().
        canvas = FindObjectOfType<LobbyCanvas>();
        canvas.initialize();

        // UI
        canvas.updateConnectedPanel(readyPlayers);
        canvas.initHostPanel(IsHost);

            if (IsServer)
        {
            // Call callbacks to track connections
            NetworkingManager.Singleton.OnClientConnectedCallback += updateConnections;
            NetworkingManager.Singleton.OnClientDisconnectCallback += updateConnections;
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
    public void OnDisconnectButton()
    {
        if (NetworkingManager.Singleton.IsConnectedClient)
        {
            // Disconnect ourselves
            NetworkingManager.Singleton.DisconnectClient(NetworkingManager.Singleton.LocalClientId);
        }
        // Destroy old network
        Destroy(GameObject.FindGameObjectWithTag("Network"));
        // Go back
        SceneManager.LoadScene("Connection");
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

        canvas.updateConnectedPanel(readyPlayers);
        InvokeClientRpcOnEveryone(UpdateConnectedOnClients, connected, canvas.playersText.text);

        if (readyPlayers.Count >= 2)
        {
            // Enough players are ready, launch with delay
            startGame();
        }
    }

    // Server-Side, notifies clients and queues scene switch
    private void startGame()
    {
        int delay = 5;
        InvokeClientRpcOnEveryone(StartCountdownOnClient, delay);
        Invoke("SwitchScene", delay);
    }

    // Server-Side, called by connect/disconnect callbacks
    public void updateConnections(ulong clientId)
    {
        connected = NetworkingManager.Singleton.ConnectedClientsList.Count;

        // Reset readiness
        if (readyPlayers.Contains(clientId))
        {
            readyPlayers.Remove(clientId);
        }

        canvas.updateConnectedPanel(readyPlayers);
        InvokeClientRpcOnEveryone(UpdateConnectedOnClients, connected, canvas.playersText.text);
    }   

    [ClientRPC]
    public void UpdateConnectedOnClients(int connected, string text)
    {
        this.connected = connected;

        // UI
        canvas.updateConnectedPanel(text);
        canvas.botButton.interactable = connected < 2; // Bot button disabled if there are two or more players
    }


    // A client wants to play with bot
    [ServerRPC(RequireOwnership = false)]
    public void StartBotModeOnServer()
    {
        GameObject botFlag = new GameObject();
        botFlag.tag = "BotFlag";
        DontDestroyOnLoad(botFlag);

        // Start Game
        startGame();
    }

    //--------------------------------------------
    // Dropdown
    //--------------------------------------------

    // Client tells server the dropdown value and server sends it to all clients
    public void OnDropdownChanged()
    {
        if (IsServer)
        {
            InvokeClientRpcOnEveryone(SetDropdownValueOnClient, canvas.levelDropdown.value);
        }
        else
        {
            InvokeServerRpc(SendDropdownValueToServer, canvas.levelDropdown.value);
        }
    }

    // Client recieves new dropdown value from the server
    [ClientRPC]
    public void SetDropdownValueOnClient(int value)
    {
        canvas.levelDropdown.SetValueWithoutNotify(value);

        // TODO: Set level preview based on dropdown value
        // canvas.levelPreview = 
    }

    // Server recieves new dropdown value from a client
    [ServerRPC(RequireOwnership = false)]
    public void SendDropdownValueToServer(int value)
    {
        canvas.levelDropdown.value = value;
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
        NetworkingManager.Singleton.OnClientConnectedCallback -= updateConnections;
        NetworkingManager.Singleton.OnClientDisconnectCallback -= updateConnections;

        NetworkSceneManager.SwitchScene(canvas.levelDropdown.captionText.text);
    }
}
