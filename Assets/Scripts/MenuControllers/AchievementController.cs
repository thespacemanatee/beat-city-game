using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    public int numPlayers = 4; 
    
    // public IntVariable[] PlayerStats;
    public IntVariable player1Stats;
    public IntVariable player2Stats;
    public IntVariable player3Stats;
    public IntVariable player4Stats;

    public GameObject[] MostIcons;

    public GameObject[] LeastIcons;

    int maxValue;
    int minValue;
    private bool allowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Achievement Controller start top, playerstats length: " + PlayerStats.Length);
        // foreach (IntVariable i in PlayerStats) {
        //     Debug.Log("IntVariable: " + i);
        //     Debug.Log("Count: " + i.Value);
        // }
        Debug.Log("Player 1 stats intvariable: " + player1Stats);
        Debug.Log("Player 1 stats value: " + player1Stats.Value);
        Debug.Log("Player 2 stats intvariable: " + player2Stats);
        Debug.Log("Player 2 stats value: " + player2Stats.Value);
        Debug.Log("Player 3 stats intvariable: " + player3Stats);
        Debug.Log("Player 3 stats value: " + player3Stats.Value);
        Debug.Log("Player 4 stats intvariable: " + player4Stats);
        Debug.Log("Player 4 stats value: " + player4Stats.Value);
        // testSetStats();
        getMaxMin();
        Debug.Log("Max value: " + maxValue);
        Debug.Log("Min value: " + minValue);
        updateIcons();
        // make sure all UI is updated first
        displayIcons();
        // Debug.Log("Achievement Controller start bottom, playerstats length: " + PlayerStats.Length);
    }
    
    void getMaxMin(){
        // Get the max and min value from the scriptable objects
        maxValue = Mathf.Max(player1Stats.Value, player2Stats.Value);
        maxValue = Mathf.Max(player3Stats.Value, maxValue);
        maxValue = Mathf.Max(player4Stats.Value, maxValue);

        minValue = Mathf.Min(player1Stats.Value, player2Stats.Value);
        minValue = Mathf.Min(player3Stats.Value, minValue);
        minValue = Mathf.Min(player4Stats.Value, minValue);
        // maxValue = PlayerStats[0].Value;
        // minValue = PlayerStats[0].Value;
        // for (int i = 1; i<numPlayers; i++)
        // {
        //     if (PlayerStats[i].Value > maxValue)
        //     {
        //         maxValue = PlayerStats[i].Value;
        //     }
        //     if (PlayerStats[i].Value < minValue){
        //         minValue = PlayerStats[i].Value;
        //     }
        // }
    }

    void updateIcon(IntVariable playerStats, GameObject mostIcon, GameObject leastIcon) {
        if (playerStats.Value != maxValue) {
            setIconBrightness(mostIcon);
        }

        if (playerStats.Value != minValue) {
            setIconBrightness(leastIcon);
        }
    }

    // Update icons that should be 
    void updateIcons()
    {   
        // Loop through and set the brightness of the highest 
        updateIcon(player1Stats,MostIcons[0], LeastIcons[0]);
        updateIcon(player2Stats,MostIcons[1], LeastIcons[1]);
        updateIcon(player3Stats,MostIcons[2], LeastIcons[2]);
        updateIcon(player4Stats,MostIcons[3], LeastIcons[3]);
        // for (int i = 0; i<numPlayers; i++)
        // {
        //     if (PlayerStats[i].Value != maxValue)
        //     {
        //         setIconBrightness(MostIcons[i]);
        //     }

        //     if (PlayerStats[i].Value != minValue)
        //     {
        //         setIconBrightness(LeastIcons[i]);
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
    //     PlayerStats[0].SetValue(0);
    //     PlayerStats[1].SetValue(1);
    //     PlayerStats[2].SetValue(3);
    //     PlayerStats[3].SetValue(3);
    // }

    void displayIcons(){
        foreach(Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
    }
    
}
