using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckLastMan : MonoBehaviour
{
    public CustomEvent winEvent;

    private bool gameEnded;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length < 2)
        {
            gameEnded = true;
            var lastMan = GameObject.FindGameObjectsWithTag("Player")[0];
            winEvent.Invoke(lastMan.name);
        }

        if ((Input.GetButtonDown("Player1_Jump") || Input.GetButtonDown("Player2_Jump") ||
             Input.GetButtonDown("Player3_Jump") || Input.GetButtonDown("Player4_Jump")) && gameEnded)
        {
            Debug.Log("restart pressed");
            SceneManager.LoadScene("BeatCity");
        }
    }
}