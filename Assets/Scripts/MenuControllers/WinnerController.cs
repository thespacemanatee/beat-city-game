using UnityEngine;
using UnityEngine.UI;

public class WinnerController : MonoBehaviour
{
    public int numPlayers = 4; 
    
    public IntVariable[] PlayerWinStats;
    public IntVariable[] PlayerEnergyStats;
    public IntVariable[] PlayerKillStats;


    public GameObject[] PlayerIcons;

    public GameObject[] CrownIcons;

    int maxValue;
    private bool allowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        testSetStats();
        getMax();
        updateIcons();
        // make sure all UI is updated first
        displayIcons();
    }
    
    void getMax(){
        // Get the max and min value from the scriptable objects
        maxValue = PlayerWinStats[0].Value;
        for (int i = 1; i<numPlayers; i++)
        {
            if (PlayerWinStats[i].Value > maxValue)
            {
                maxValue = PlayerWinStats[i].Value;
            }
        }
    }

    // Update icons that should be 
    void updateIcons()
    {   
        // Loop through and set the brightness of the highest 
        for (int i = 0; i<numPlayers; i++)
        {
            if (PlayerWinStats[i].Value != maxValue)
            {
                setIconBrightness(PlayerIcons[i]);
                CrownIcons[i].GetComponent<Image>().CrossFadeAlpha(0,0, true);
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
        // PlayerWinStats[0].SetValue(3);
        // PlayerWinStats[1].SetValue(1);
        // PlayerWinStats[2].SetValue(2);
        // PlayerWinStats[3].SetValue(0);
       
        PlayerWinStats[0].SetValue(0);
        PlayerWinStats[1].SetValue(3);
        PlayerWinStats[2].SetValue(3);
        PlayerWinStats[3].SetValue(3);
       
        // PlayerWinStats[0].SetValue(0);
        // PlayerWinStats[1].SetValue(1);
        // PlayerWinStats[2].SetValue(3);
        // PlayerWinStats[3].SetValue(2);
       
        // PlayerWinStats[0].SetValue(0);
        // PlayerWinStats[1].SetValue(1);
        // PlayerWinStats[2].SetValue(2);
        // PlayerWinStats[3].SetValue(3);
    }

    public void resetAllVariables()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerKillStats[i].SetValue(0);            
            PlayerWinStats[i].SetValue(0);            
            PlayerEnergyStats[i].SetValue(0);            
        }
    }

    void displayIcons(){
        foreach(Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
    }
    
}
