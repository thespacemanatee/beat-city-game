using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinEventController : MonoBehaviour
{
    public string playerId;
    public GameObject WinnerScreen;
    private bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreenActive(string PlayerName){
        Debug.Log("Winner active");
        Debug.Log(PlayerName);
        if (PlayerName == playerId && !activated)
        {
            WinnerScreen.SetActive(true);
            activated = true;
        }
    }
}
