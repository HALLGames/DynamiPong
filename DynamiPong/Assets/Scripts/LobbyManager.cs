using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkedBehaviour
{
    // UI
    public Text connectText;
    public Text readyText;
    public Dropdown levelDropdown;
    public Text countdownText;
    public Button readyButton;
    public Button botButton;

    // Keep track of connections on Server
    private int connected;
    private int ready;
    private bool localReady;

    // Start is called before the first frame update
    public override void NetworkStart()
    {
        if (IsServer)
        {
            NetworkingManager.Singleton.OnClientConnectedCallback += UpdateConnected;
            NetworkingManager.Singleton.OnClientDisconnectCallback += UpdateConnected;
        }
    }

    //--------------------------------------------
    // Buttons
    //--------------------------------------------

    // Ready Button Clicked
    public void OnReadyButton()
    {
        // Toggle readiness
        localReady = !localReady;

        // Updated UI - Button becomes cancel button
        // readyButton.GetComponentInChildren<Text>().text = localReady ? "Cancel" : "Ready";

        InvokeServerRpc(NotifyReadyForServer, localReady);
    }

    // Bot Button Clicked
    public void OnBotButton()
    {
        // InvokeServerRpc(SendBot);
    }

    //--------------------------------------------
    // Connected and Ready
    //--------------------------------------------

    [ServerRPC(RequireOwnership = false)]
    public void NotifyReadyForServer(bool isReady)
    {
        ready += isReady ? 1 : -1;

        if (ready >= 2 && connected >= 2)
        {
            // Enough players are ready
            // InvokeClientRpcOnEveryone(StartCountdown, 5);
            Invoke("SwitchScene", 5);
        }
    }

    public void UpdateConnected(ulong clientId)
    {
        connected = NetworkingManager.Singleton.ConnectedClients.Count;
        InvokeClientRpcOnEveryone(UpdatedConnectedOnClients, connected, ready);
    }

    [ClientRPC]
    public void UpdatedConnectedOnClients(int connected, int ready)
    {
        this.connected = connected;
        this.ready = ready;
        UpdateConnectedAndReadyUI();
    }

    private void UpdateConnectedAndReadyUI()
    {
        // connectText.text = "Connected: " + connected;
        // readyText.text = "Ready: " + ready;
    }

    //--------------------------------------------
    // Dropdown
    //--------------------------------------------

    // Client tells server the dropdown value and server sends it to all clients
    public void OnDropdownChanged()
    {
        if (IsServer)
        {
            // InvokeClientRpcOnEveryone(SetDropdownValue, levelDropdown.value);
        }
        else
        {
            // InvokeServerRpc(SendDropdownValue, levelDropdown.value);
        }
    }

    // Client recieves new dropdown value from the server
    [ClientRPC]
    public void SetDropdownValue(int value)
    {
        // levelDropdown.SetValueWithoutNotify(value);
    }

    // Server recieves new dropdown value from a client
    [ServerRPC(RequireOwnership = false)]
    public void SendDropdownValue(int value)
    {
        // levelDropdown.value = value;
    }

    //--------------------------------------------
    // Countdown and Scene Switch
    //--------------------------------------------

    // Initiates scene switch and countdown (client side)
    [ClientRPC]
    public void StartCountdown(int delay)
    {
        StartCoroutine(countdown(5));

        // Disable UI after countdown starts
        // readyButton.interactable = false;
        // botButton.interactable = false;
        // levelDropdown.interactable = false;
    }

    // Switches level after a countdown of "delay" seconds
    private IEnumerator countdown(int delay)
    {
        for (int i = delay - 1; i > 0; i--)
        {
            // Update the countdown text
            // countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
    }

    // Server switches the Scene based on Dropdown selection
    public void SwitchScene()
    {
        // NetworkSceneManager.SwitchScene(levelDropdown.captionText.text);
    }

    

}
