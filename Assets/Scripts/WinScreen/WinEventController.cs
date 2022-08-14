using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WinEventController : MonoBehaviour
{
    public string playerId;
    public GameObject WinnerScreen;
    public GameObject DrawScreen;
    private bool activated;
    public GameConstants gameConstants;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetScreenActive(string PlayerName)
    {
        // Add delay time here 
        if (PlayerName == playerId && !activated)
        {
            activated = true;
            StartCoroutine(DelayBeforeShow());
        }
    }

    public void SetDrawScreenActive() {
        StartCoroutine(DelayBeforeShowDraw());
    }

    IEnumerator DelayBeforeShow()
    {
        yield return new WaitForSeconds(1.5f);
        WinnerScreen.SetActive(true);
    }

    IEnumerator DelayBeforeShowDraw()
    {
        yield return new WaitForSeconds(1.5f);
        DrawScreen.SetActive(true);
    }
}