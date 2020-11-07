using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerBars : MonoBehaviour
{
    public Text leftPlayerName;
    public Text rightPlayerName;
    public Text leftPlayerScore;
    public Text rightPlayerScore;
    public Text leftPlayerReady;
    public Text rightPlayerReady;

    public void clear()
    {
        leftPlayerName.text = "";
        rightPlayerName.text = "";
        leftPlayerScore.text = "";
        rightPlayerScore.text = "";
        leftPlayerReady.gameObject.SetActive(false);
        rightPlayerReady.gameObject.SetActive(false);
    }

    public void addPlayer(string player, string score, bool ready, int playerNumber)
    {
        if (playerNumber == 0)
        {
            leftPlayerName.text = player;
            leftPlayerScore.text = score;
            leftPlayerReady.gameObject.SetActive(ready);
        } else if (playerNumber == 1)
        {
            rightPlayerName.text = player;
            rightPlayerScore.text = score;
            rightPlayerReady.gameObject.SetActive(ready);
        }
    }

    public void setPlayerNames(string leftName, string rightName)
    {
        leftPlayerName.text = leftName;
        rightPlayerName.text = rightName;
    }

    public void setPlayerScores(string leftScore, string rightScore)
    {
        leftPlayerScore.text = leftScore;
        rightPlayerScore.text = rightScore;
    }

    public void setReady(bool leftReady, bool rightReady)
    {
        leftPlayerReady.gameObject.SetActive(leftReady);
        rightPlayerReady.gameObject.SetActive(rightReady);
    }

    public string getLeftPlayerName()
    {
        return leftPlayerName.text;
    }

    public string getRightPlayerName()
    {
        return rightPlayerName.text;
    }

    public string getLeftPlayerScore()
    {
        return leftPlayerScore.text;
    }

    public string getRightPlayerScore()
    {
        return rightPlayerScore.text;
    }

    public bool getLeftPlayerReady()
    {
        return leftPlayerReady.gameObject.activeSelf;
    }

    public bool getRightPlayerReady()
    {
        return rightPlayerReady.gameObject.activeSelf;
    }
}
