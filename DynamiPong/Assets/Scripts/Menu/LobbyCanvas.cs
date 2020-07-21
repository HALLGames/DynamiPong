using MLAPI;
using MLAPI.Connection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviour
{
    // UI
    public Text playersText;
    public Dropdown levelDropdown;
    public Image levelPreview;
    public RectTransform countdownPanel;
    public Text countdownText;
    public Button readyButton;
    public Button botButton;

    private Network network;

    public void initialize()
    {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();

        playersText.text = "";
        countdownPanel.gameObject.SetActive(false);
        countdownText.text = string.Empty;

        // TODO: Auto-init the dropdown with levels

        // TODO: Level previews
        // levelPreview = 
    }

    public void updateReadyButton(bool localReady)
    {
        // Ready button toggles with cancel button
        readyButton.GetComponentInChildren<Text>().text = localReady ? "Cancel" : "Ready";
    }

    /// <summary>
    /// Adds all connected player names to panel.
    /// Players who are ready will have "(Ready)" appear after their name.
    /// </summary>
    /// <param name="readyList"></param>
    public void updateConnectedPanel(List<ulong> readyList)
    {
        // Reset Text
        playersText.text = "";

        // Add all client names
        foreach (NetworkedClient client in NetworkingManager.Singleton.ConnectedClientsList)
        {
            ulong id = client.ClientId;
            string name;
            if (network.getConnectedPlayerNames().ContainsKey(id))
            {
                name = network.getConnectedPlayerNames()[id];
            } 
            else
            {
                name = "Unknown";
            }
            
            if (readyList.Contains(id))
            {
                name += " (Ready)";
            }

            playersText.text += name + "\n";
        } 
    }

    public void startCountdown(int delay)
    {
        countdownPanel.gameObject.SetActive(true);
        StartCoroutine(countdown(delay));

        // Disable UI after countdown starts
        readyButton.interactable = false;
        botButton.interactable = false;
        levelDropdown.interactable = false;
    }

    // Switches level after a countdown of "delay" seconds
    private IEnumerator countdown(int delay)
    {
        for (int i = delay - 1; i > 0; i--)
        {
            // Update the countdown text
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        countdownPanel.gameObject.SetActive(false);
    }

}
