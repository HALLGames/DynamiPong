using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBars : MonoBehaviour
{
    public Text leftPlayerText;
    public Text rightPlayerText;
    public Text leftPlayerScore;
    public Text rightPlayerScore;

    public void initialize(string leftPlayerName, string rightPlayerName)
    {
        leftPlayerText.text = leftPlayerName;
        rightPlayerText.text = rightPlayerName;
        leftPlayerScore.text = "0";
        rightPlayerScore.text = "0";
    }

    public void updateScore(int leftScore, int rightScore)
    {
        leftPlayerScore.text = leftScore.ToString();
        rightPlayerScore.text = rightScore.ToString();
    }
}
