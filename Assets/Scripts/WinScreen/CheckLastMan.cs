using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckLastMan : MonoBehaviour
{
    public IntVariable Player1WinCount;
    public IntVariable Player2WinCount;
    public IntVariable Player3WinCount;
    public IntVariable Player4WinCount;
    
    public CustomEvent winEvent;
    private bool gameEnded;
    private string playerId;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length < 2 && !gameEnded)
        {
            gameEnded = true;
            var lastMan = GameObject.FindGameObjectsWithTag("Player")[0];
            updateWinner(lastMan.name);
            winEvent.Invoke(playerId);
        }

        // if ANY player presses jump then go to next scene
        if ((Input.GetButtonDown("Player1_Jump") || Input.GetButtonDown("Player2_Jump") ||
             Input.GetButtonDown("Player3_Jump") || Input.GetButtonDown("Player4_Jump")) && gameEnded)
        {
            SceneManager.LoadScene("BeatCity"); // change this to other scene for different levels
        }
    }

    public void updateWinner(string winner)
    {
        Debug.Log("Winner for this round is: " + winner);
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
}