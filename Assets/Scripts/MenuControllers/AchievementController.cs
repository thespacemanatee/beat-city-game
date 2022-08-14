using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    public int numPlayers = 4; 
    
    public IntVariable[] PlayerStats;

    public GameObject[] MostIcons;

    public GameObject[] LeastIcons;

    int maxValue;
    int minValue;
    private bool allowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        // testSetStats();
        getMaxMin();
        updateIcons();
        // make sure all UI is updated first
        displayIcons();
    }
    
    void getMaxMin(){
        // Get the max and min value from the scriptable objects
        maxValue = PlayerStats[0].Value;
        minValue = PlayerStats[0].Value;
        for (int i = 1; i<numPlayers; i++)
        {
            if (PlayerStats[i].Value > maxValue)
            {
                maxValue = PlayerStats[i].Value;
            }
            if (PlayerStats[i].Value < minValue){
                minValue = PlayerStats[i].Value;
            }
        }
    }

    // Update icons that should be 
    void updateIcons()
    {   
        // Loop through and set the brightness of the highest 
        for (int i = 0; i<numPlayers; i++)
        {
            if (PlayerStats[i].Value != maxValue)
            {
                setIconBrightness(MostIcons[i]);
            }

            if (PlayerStats[i].Value != minValue)
            {
                setIconBrightness(LeastIcons[i]);
            }
        }
    }

    // 'darken' the noobs so we know who actually won
    void setIconBrightness(GameObject icon)
    {
        float brightness = 0.25f;
        icon.GetComponent<Image>().color = new Color(brightness,brightness,brightness);
    }

    void testSetStats(){
        PlayerStats[0].SetValue(0);
        PlayerStats[1].SetValue(1);
        PlayerStats[2].SetValue(3);
        PlayerStats[3].SetValue(3);
    }

    void displayIcons(){
        foreach(Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
    }
    
}
