using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCanvasBehaviour : MonoBehaviour
{
    // UI
    public PlayerScoreBars playerScores;
    public Text winConText;
    public Button concedeButton;
    public GameTimer timer;
    public VictoryPanel victoryPanelPrefab;

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
    /// Override this function when you add something to be disabled
    /// </summary>
    public virtual void disableUI()
    {
        concedeButton.interactable = false;
        timer.stop();
    }

    public void initWinConText(GameInfo.WinCondition winCon)
    {
        string text = "Win Condition: ";
        switch (winCon)
        {
            case GameInfo.WinCondition.Freeplay:
                text = "Freeplay";
                break;
            case GameInfo.WinCondition.FirstTo10:
                text += "First To 10 Points";
                break;
            case GameInfo.WinCondition.FirstTo20:
                text += "First To 20 Points";
                break;
            case GameInfo.WinCondition.MostAfter5:
                text += "Most Points After 5 Minutes";
                timer.start(5 * 60);
                break;
            case GameInfo.WinCondition.MostAfter10:
                text += "Most Points After 10 Minutes";
                timer.start(10 * 60);
                break;
        }
        winConText.text = text;
    }

    public void showVictoryPanel(GameManagerBehaviour.GameWinState winState)
    {
        VictoryPanel victoryPanel = Instantiate(victoryPanelPrefab, transform);
        switch(winState)
        {
            case GameManagerBehaviour.GameWinState.Win:
                victoryPanel.setupVictory();
                break;
            case GameManagerBehaviour.GameWinState.Loss:
                victoryPanel.setupLoss();
                break;
            case GameManagerBehaviour.GameWinState.Tie:
                victoryPanel.setupTie();
                break;
        }

        disableUI();
        victoryPanel.continueButton.onClick.AddListener(victoryPanelContinueButton);
    }

    public void victoryPanelContinueButton()
    {
        GameManagerBehaviour gameManager = FindObjectOfType<GameManagerBehaviour>();
        gameManager.InvokeServerRpc(gameManager.WaitForClientsBeforeEnd);
    }
}
