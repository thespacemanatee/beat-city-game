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
            winEvent.Invoke(lastMan.name);
            updateWinner(lastMan.name);
        }

        if ((Input.GetButtonDown("Player1_Jump") || Input.GetButtonDown("Player2_Jump") ||
             Input.GetButtonDown("Player3_Jump") || Input.GetButtonDown("Player4_Jump")) && gameEnded)
        {
            Debug.Log("restart pressed");
            SceneManager.LoadScene("BeatCity");
        }
    }

    public void updateWinner(string winner)
    {
        switch (winner)
        {
            case "MinimalCharacter":
                Player1WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 1":
                Player2WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 2":
                Player3WinCount.ApplyChange(1);
                break;
            case "MinimalCharacter 3":
                Player4WinCount.ApplyChange(1);
                break;
            default:
                Debug.Log("INCORRECT PLAYER ID");
                break;
        }
    }
}