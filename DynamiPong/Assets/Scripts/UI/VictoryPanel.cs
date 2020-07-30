using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    public Text title;
    public Text message;
    public Button continueButton;

    // Start is called before the first frame update
    void Start()
    {
        continueButton.onClick.AddListener(disableContinueButton);
    }

    public void setupVictory()
    {
        title.text = "Victory!";
        message.text = "Great job, you won!" + "\nClick continue to return to lobby.";
    }

    public void setupLoss()
    {
        title.text = "Loss!";
        message.text = "You lost. Better luck next time." + "\nClick continue to return to lobby.";
    }

    public void setupTie()
    {
        title.text = "Tie!";
        message.text = "Its a tie. You know what they say: Great Minds Play Alike!" + "\nClick continue to return to lobby.";
    }

    public void disableContinueButton()
    {
        continueButton.interactable = false;
        continueButton.GetComponentInChildren<Text>().text = "Waiting for Other Player...";
    }
}
