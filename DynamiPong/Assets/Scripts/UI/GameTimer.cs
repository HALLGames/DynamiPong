using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText;

    // Callback
    [HideInInspector]
    public Action TimerFinishedCallback;

    private bool isRunning = false;
    private float timeRemaining = 0;

    // Used for text display
    private int secondsRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        timerText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                // Only update text if it should be changed
                int newSecondsRemaining = Mathf.FloorToInt(timeRemaining + 1);
                if (newSecondsRemaining != secondsRemaining)
                {
                    secondsRemaining = newSecondsRemaining;
                    updateTimeText();
                }
            } else
            {
                timeRemaining = 0;
                isRunning = false;
                TimerFinishedCallback?.Invoke();
            }
        }
    }

    private void updateTimeText()
    {
        float timeToDisplay = timeRemaining + 1;
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void start(float seconds)
    {
        timeRemaining = seconds;
        isRunning = true;
    }

    public void stop()
    {
        isRunning = false;
    }

    public void resume()
    {
        if (timeRemaining > 0)
        {
            isRunning = true;
        }
    }

    public bool isTimerRunning()
    {
        return isRunning;
    }

    public float getTimeRemaining()
    {
        return timeRemaining;
    }
}
