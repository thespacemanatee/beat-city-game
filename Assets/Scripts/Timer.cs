using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameConstants gameConstants;
    public bool timerIsRunning;
    public Text timeText;
    public UnityEvent onTimerFired;
    private float _dropCutoffTime;
    private float _timeRemaining;

    private void Start()
    {
        // Starts the timer automatically
        _timeRemaining = gameConstants.gameDurationInSeconds;
        timerIsRunning = true;
        _dropCutoffTime = _timeRemaining * gameConstants.shrinkMapIntervalMultiplier;
    }

    private void Update()
    {
        if (timerIsRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                // Calculate whether tiles should be dropped
                if (_timeRemaining < _dropCutoffTime)
                {
                    onTimerFired.Invoke();
                    if (_dropCutoffTime * (1 - gameConstants.shrinkMapIntervalMultiplier) >= gameConstants.minShrinkMapIntervalInSeconds)
                    {
                        _dropCutoffTime *= gameConstants.shrinkMapIntervalMultiplier;
                    }
                    else
                    {
                        _dropCutoffTime -= gameConstants.minShrinkMapIntervalInSeconds;
                    }
                }
            }
            else
            {
                Debug.Log("Time has run out!");
                _timeRemaining = 0;
                _dropCutoffTime = 0;
                timerIsRunning = false;
            }

            DisplayTime(_timeRemaining);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
        var milliSeconds = timeToDisplay % 1 * 1000;
        timeText.text = $"{minutes:00}:{seconds:00}:{milliSeconds:000}";
    }
}