using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public GameConstants gameConstants;
    public bool timerIsRunning = false;
    public Text timeText;
    public UnityEvent onTimerFired;
    private float timeRemaining;
    private float dropCutoffTime;
    private void Start()
    {
        // Starts the timer automatically
        timeRemaining = gameConstants.gameDurationInSeconds;
        timerIsRunning = true;
        dropCutoffTime = timeRemaining * 0.75f;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                // Calculate whether tiles should be dropped
                if(timeRemaining < dropCutoffTime){
                    onTimerFired.Invoke();
                    dropCutoffTime = dropCutoffTime * 0.75f;
                }
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                dropCutoffTime = 0;
                timerIsRunning = false;

            }
            DisplayTime(timeRemaining);

        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        float milliSeconds = (timeToDisplay % 1) * 1000;
        timeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);

    }
}
