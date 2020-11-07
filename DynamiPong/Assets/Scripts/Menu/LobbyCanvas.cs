using MLAPI;
using MLAPI.Connection;
using MLAPI.Transports.UNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviour
{
    // UI
    public LobbyPlayerBars playerBars;
    public Dropdown levelDropdown;
    public Dropdown winConDropdown;
    public Image levelPreview;
    public Button readyButton;
    public Button botButton;
    public RectTransform hostPanel;
    public Text hostText;
    public RectTransform countdownPanel;
    public Text countdownText;

    private Network network;
    private AudioSource countdownAudio;

    public void initialize()
    {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();

        playerBars.clear();
        countdownPanel.gameObject.SetActive(false);
        countdownText.text = string.Empty;

        initWinConDropdown();
    }

    private void initWinConDropdown()
    {
        winConDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Loop over WinCondition enum and add custom strings for each
        foreach (GameInfo.WinCondition winCon in Enum.GetValues(typeof(GameInfo.WinCondition)))
        {
            string optionText = null;
            switch (winCon)
            {
                case GameInfo.WinCondition.Freeplay:
                    optionText = "Freeplay (no limit)";
                    break;
                case GameInfo.WinCondition.FirstTo10:
                    optionText = "First to 10 pts.";
                    break;
                case GameInfo.WinCondition.FirstTo20:
                    optionText = "First to 20 pts.";
                    break;
                case GameInfo.WinCondition.MostAfter5:
                    optionText = "Most after 5 min.";
                    break;
                case GameInfo.WinCondition.MostAfter10:
                    optionText = "Most after 10 min.";
                    break;
            }
            if (optionText != null)
            {
                options.Add(optionText);
            }
        }
        winConDropdown.AddOptions(options);
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
    public void updatePlayerBarsOnServer(List<ulong> readyList)
    {
        playerBars.clear();
        int playerNum = 0;

        // Add all client names
        foreach (NetworkedClient client in NetworkingManager.Singleton.ConnectedClientsList)
        {
            string name;
            int playerScore;
            bool ready;
            ulong id = client.ClientId;

            if (network.getConnectedPlayerNames().ContainsKey(id))
            {
                name = network.getConnectedPlayerNames()[id];
            } 
            else
            {
                name = "Unknown";
            }

            if (network.connectedPlayerScores.ContainsKey(id))
            {
                playerScore = network.connectedPlayerScores[id];
            } else
            {
                playerScore = 0;
            }

            ready = readyList.Contains(id);

            playerBars.addPlayer(name, playerScore.ToString(), ready, playerNum);

            playerNum++;
        }
    }

    public void startCountdown(int delay)
    {
        countdownPanel.gameObject.SetActive(true);
        StartCoroutine(countdown(delay));

        // Disable UI after countdown starts
        disableUI();

        // Fade out music
        StartCoroutine(fadeMusic(2));
    }

    // Switches level after a countdown of "delay" seconds
    private IEnumerator countdown(int delay)
    {
        // Init Sound
        countdownAudio = gameObject.AddComponent<AudioSource>();
        countdownAudio.clip = Resources.Load<AudioClip>("Sound/Common/Countdown");

        for (int i = delay - 1; i > 0; i--)
        {
            // Update the countdown text
            countdownText.text = i.ToString();

            // Play Sound
            countdownAudio.Play();

            yield return new WaitForSeconds(1);
        }
        countdownPanel.gameObject.SetActive(false);
    }

    public void disableUI()
    {
        // Disable most buttons
        readyButton.interactable = false;
        botButton.interactable = false;
        levelDropdown.interactable = false;
        winConDropdown.interactable = false;

        // Fade out preview
        levelPreview.CrossFadeAlpha(100, 0, true);
    }

    private IEnumerator fadeMusic(float duration)
    {
        AudioSource music = GameObject.FindGameObjectWithTag("BackgroundMusic").GetComponent<AudioSource>();
        float volChangeOverTime = music.volume / duration;
        while (music.volume > 0)
        {
            music.volume -= volChangeOverTime * Time.deltaTime;
            yield return null;
        }
    }

    public void initHostPanel(bool isHost)
    {
        if (isHost)
        {
            UnetTransport transport = network.GetComponent<UnetTransport>();
            hostText.text =
                "- Hosting at - \n" +
                "Address: " + transport.ConnectAddress + "\n" +
                "Port: " + transport.ConnectPort;
        }
        else
        {
            hostPanel.gameObject.SetActive(false);
        }
    }

    public void updateLevelPreview()
    {
        levelPreview.sprite = Resources.Load<Sprite>("Images/" + levelDropdown.captionText.text);
    }
}
