using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CheckLastMan : MonoBehaviour
{
    public CustomEvent winEvent;
    bool gameEnded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Player").Length<2)
        {
            gameEnded = true;
            GameObject lastMan = GameObject.FindGameObjectsWithTag("Player")[0];
            winEvent.Invoke(lastMan.name);
        }

        if ((Input.GetButtonDown("Player1_Jump") || Input.GetButtonDown("Player2_Jump") || Input.GetButtonDown("Player3_Jump") || Input.GetButtonDown("Player4_Jump")) && gameEnded)
        {
            Debug.Log("restart pressed");
            SceneManager.LoadScene("BeatCity");
        }

    }
}
