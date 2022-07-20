using UnityEngine;

public class WinEventController : MonoBehaviour
{
    public string playerId;
    public GameObject WinnerScreen;
    private bool activated;

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
        if (PlayerName == playerId && !activated)
        {
            WinnerScreen.SetActive(true);
            activated = true;
        }
    }
}