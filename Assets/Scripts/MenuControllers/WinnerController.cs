using UnityEngine;
using UnityEngine.UI;

public class WinnerController : MonoBehaviour
{
    public int numPlayers = 4; 
    public IntVariable player1WinStats;
    public IntVariable player2WinStats;
    public IntVariable player3WinStats;
    public IntVariable player4WinStats;

    public IntVariable player1EnergyStats;
    public IntVariable player2EnergyStats;
    public IntVariable player3EnergyStats;
    public IntVariable player4EnergyStats;

    public IntVariable player1KillStats;
    public IntVariable player2KillStats;
    public IntVariable player3KillStats;
    public IntVariable player4KillStats;
    // public IntVariable[] PlayerWinStats;
    // public IntVariable[] PlayerEnergyStats;
    // public IntVariable[] PlayerKillStats;


    public GameObject[] PlayerIcons;

    public GameObject[] CrownIcons;

    int maxValue;
    private bool allowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        // testSetStats();
        // Debug.Log("WinnerController start (top), PlayerWinStats length: " + PlayerWinStats.Length);
        getMax();
        updateIcons();
        // make sure all UI is updated first
        displayIcons();
        // Debug.Log("WinnerController start (bottom), PlayerWinStats length: " + PlayerWinStats.Length);
    }
    
    void getMax(){
        // Get the max and min value from the scriptable objects
        // maxValue = PlayerWinStats[0].Value;
        // for (int i = 1; i<numPlayers; i++)
        // {
        //     if (PlayerWinStats[i].Value > maxValue)
        //     {
        //         maxValue = PlayerWinStats[i].Value;
        //     }
        // }

        maxValue = Mathf.Max(player1WinStats.Value, player2WinStats.Value);
        maxValue = Mathf.Max(player3WinStats.Value, maxValue);
        maxValue = Mathf.Max(player4WinStats.Value, maxValue);
    }

    void updateIcon(IntVariable playerStats, GameObject icon, GameObject crownIcon) {
        if (playerStats.Value != maxValue) {
            setIconBrightness(icon);
            crownIcon.GetComponent<Image>().CrossFadeAlpha(0,0, true);
        }
    }

    // Update icons that should be 
    void updateIcons()
    {   
        updateIcon(player1WinStats, PlayerIcons[0], CrownIcons[0]);
        updateIcon(player1WinStats, PlayerIcons[1], CrownIcons[1]);
        updateIcon(player1WinStats, PlayerIcons[2], CrownIcons[2]);
        updateIcon(player1WinStats, PlayerIcons[3], CrownIcons[3]);
        // // Loop through and set the brightness of the highest 
        // for (int i = 0; i<numPlayers; i++)
        // {
        //     if (PlayerWinStats[i].Value != maxValue)
        //     {
        //         setIconBrightness(PlayerIcons[i]);
        //         CrownIcons[i].GetComponent<Image>().CrossFadeAlpha(0,0, true);
        //     }
        // }
    }

    // 'darken' the noobs so we know who actually won
    void setIconBrightness(GameObject icon)
    {
        float brightness = 0.25f;
        icon.GetComponent<Image>().color = new Color(brightness,brightness,brightness);
    }

    // void testSetStats(){
    //     // PlayerWinStats[0].SetValue(3);
    //     // PlayerWinStats[1].SetValue(1);
    //     // PlayerWinStats[2].SetValue(2);
    //     // PlayerWinStats[3].SetValue(0);
       
    //     PlayerWinStats[0].SetValue(0);
    //     PlayerWinStats[1].SetValue(3);
    //     PlayerWinStats[2].SetValue(3);
    //     PlayerWinStats[3].SetValue(3);
       
    //     // PlayerWinStats[0].SetValue(0);
    //     // PlayerWinStats[1].SetValue(1);
    //     // PlayerWinStats[2].SetValue(3);
    //     // PlayerWinStats[3].SetValue(2);
       
    //     // PlayerWinStats[0].SetValue(0);
    //     // PlayerWinStats[1].SetValue(1);
    //     // PlayerWinStats[2].SetValue(2);
    //     // PlayerWinStats[3].SetValue(3);
    // }

    void resetStats(IntVariable playerKillStat, IntVariable playerWinStat, IntVariable playerEnergyStat) {
        playerKillStat.SetValue(0);
        playerWinStat.SetValue(0);
        playerEnergyStat.SetValue(0);
    }

    public void resetAllVariables()
    {
        Debug.Log("resetting all variables");
        resetStats(player1KillStats, player1WinStats, player1EnergyStats);
        resetStats(player2KillStats, player2WinStats, player2EnergyStats);
        resetStats(player3KillStats, player3WinStats, player3EnergyStats);
        resetStats(player4KillStats, player4WinStats, player4EnergyStats);
        // for (int i = 0; i < numPlayers; i++)
        // {
        //     PlayerKillStats[i].SetValue(0);            
        //     PlayerWinStats[i].SetValue(0);            
        //     PlayerEnergyStats[i].SetValue(0);            
        // }
        // Debug.Log("WinnerController: numPlayers: " + numPlayers);
        // for (int i = 0; i < numPlayers; i++)
        // {
        //     Debug.Log(PlayerKillStats[i]);            
        //     Debug.Log(PlayerKillStats[i].Value);            
        //     Debug.Log(PlayerWinStats[i]);            
        //     Debug.Log(PlayerWinStats[i].Value);            
        //     Debug.Log(PlayerEnergyStats[i]);            
        //     Debug.Log(PlayerEnergyStats[i].Value);            
        // }
    }

    void displayIcons(){
        foreach(Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
    }
    
}
