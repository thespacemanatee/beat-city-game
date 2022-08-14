using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using MoreMountains.TopDownEngine;

public class CheckLastMan : MonoBehaviour
{
    public IntVariable Player1WinCount;
    public IntVariable Player2WinCount;
    public IntVariable Player3WinCount;
    public IntVariable Player4WinCount;
    
    private LevelSelector levelSelector;
    private bool changingScene=false;
    public CustomEvent winEvent;
    public UnityEvent drawEvent;
    private bool gameEnded;
    private bool stopCheck = false;
    private string playerId;

    // Start is called before the first frame update
    private void Start()
    {
        levelSelector = gameObject.GetComponent<LevelSelector>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length < 2 && !gameEnded && !stopCheck)
        {
            stopCheck=true;
            StartCoroutine(DelayBeforeShow());
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0) {
                var lastMan = players[0];
                Debug.Log(lastMan.name);
                updateWinner(lastMan.name);
                winEvent.Invoke(playerId);
                Destroy(lastMan);
            } else {
                drawEvent.Invoke();
            }
        }

        // TODO: Add short delay here before checking
        // if ANY player presses jump then go to next scene
        if (gameEnded && (Input.GetButtonDown("Player1_Run") || Input.GetButtonDown("Player2_Run") ||
            Input.GetButtonDown("Player3_Run") || Input.GetButtonDown("Player4_Run")))
        {
            // SceneManager.LoadScene("BeatCity"); // change this to other scene for different levels
            if(levelSelector && !changingScene){
                changingScene=true;
                levelSelector.GoToLevel();
            }
            else{
                Debug.Log("Attach Level Selector onto LevelManager");
            }
        }
    }

    public void updateWinner(string winner)
    {
        switch (winner)
        {
            case "MinimalCharacter":
                playerId = "Player1";
                Player1WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 1":
                playerId = "Player2";
                Player2WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 2":
                playerId = "Player3";
                Player3WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 3":
                playerId = "Player4";
                Player4WinCount.ApplyChange(1);
                break;
            default:
                break;
        }
    }

    //coroutine here starts a short delay 
    IEnumerator DelayBeforeShow()
    {
        Debug.Log("gameEnded"+gameEnded);
        yield return new WaitForSeconds(1.5f);
        gameEnded = true;
        Debug.Log("gameEnded"+gameEnded);
    }
}
