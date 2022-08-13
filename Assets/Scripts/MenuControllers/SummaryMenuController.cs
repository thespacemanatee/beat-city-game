using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;

public class SummaryMenuController : MonoBehaviour
{
    public int numPlayers = 4; 
    
    public IntVariable[] PlayerWins;

    public IntVariable[] PlayerKills;

    public IntVariable[] PlayerEnergy;

    GameObject[] PlayerWinIcons;

    GameObject[] PlayerKillIcons;

    GameObject[] PlayerEnergyIcons; 

    private bool allowInput = false;
    private LevelSelector levelSelector;
    private bool changingScene=false;


    
    // Start is called before the first frame update
    void Start()
    {
        PlayerWinIcons = new GameObject[numPlayers];
        PlayerKillIcons = new GameObject[numPlayers];
        PlayerEnergyIcons = new GameObject[numPlayers];
        levelSelector = gameObject.GetComponent<LevelSelector>();
    }

    // Update is called once per frame
    void Update()
    {
            if (allowInput && (Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0) ||
             Input.GetKeyDown(KeyCode.Joystick3Button0) || Input.GetKeyDown(KeyCode.Joystick4Button0) || Input.GetKeyDown(KeyCode.Space)))
        {
            // SceneManager.LoadScene("BeatCity"); // change this to other scene for different levels
            if(levelSelector && !changingScene){
                changingScene=true;
                levelSelector.GoToLevel();
            }
            else{
                Debug.Log("Attach Level Selector onto LevelManager");
            }
        }
    }

    public void showWinMenu()
    {
        StartCoroutine(DelayBeforeShow());
    }

    // Update which icons 
    void updateIcons(GameObject[] icons,IntVariable[] variables)
    {
        // First loop through and get the highest number 
        int maxValues = 0;
        for (int i = 0; i<numPlayers; i++)
        {
            if (variables[i].Value > maxValues)
            {
                maxValues = variables[i].Value;
            }
        }
        
        // Second loop through and set the brightness of the highest 
        for (int i = 0; i<numPlayers; i++)
        {
            Debug.Log(i + "\t search" + variables[i].Value);
            if (variables[i].Value != maxValues)
            {
                setIconBrightness(icons[i]);
            }
        }
    }

    // 'darken' the noobs so we know who actually won
    void setIconBrightness(GameObject icon)
    {
        float brightness = 0.25f;
        icon.GetComponent<Image>().color = new Color(brightness,brightness,brightness);
    }

    public void restartGame()
    {
        resetAllVariables();
        // change this to other scene for different levels
        SceneManager.LoadScene("BeatCity"); 
    }

    void resetAllVariables()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerWins[i].SetValue(0);
            PlayerKills[i].SetValue(0);
            PlayerEnergy[i].SetValue(0);
        }
    }

    IEnumerator DelayBeforeShow()
    {
        yield return new WaitForSeconds(1.5f * 2);
        Time.timeScale = 0.0f;
        foreach(Transform child in transform) 
        {
            child.gameObject.SetActive(true); // or false

            // Setting win icons
            if (child.name == "Winner1Icon")
            {
                PlayerWinIcons[0] = child.gameObject;
            }
            else if (child.name == "Winner2Icon")
            {
                PlayerWinIcons[1] = child.gameObject;
            }
            else if (child.name == "Winner3Icon")
            {
                PlayerWinIcons[2] = child.gameObject;
            }
            else if (child.name == "Winner4Icon")
            {
                PlayerWinIcons[3] = child.gameObject;
            }

            // setting kills icon
            if (child.name == "MostKills1Icon")
            {
                PlayerKillIcons[0] = child.gameObject;
            }
            else if (child.name == "MostKills2Icon")
            {
                PlayerKillIcons[1] = child.gameObject;
            }
            else if (child.name == "MostKills3Icon")
            {
                PlayerKillIcons[2] = child.gameObject;
            }
            else if (child.name == "MostKills4Icon")
            {
                PlayerKillIcons[3] = child.gameObject;
            }

            // setting most energy icon
            if (child.name == "MostEnergy1Icon")
            {
                PlayerEnergyIcons[0] = child.gameObject;
            }
            else if (child.name == "MostEnergy2Icon")
            {
                PlayerEnergyIcons[1] = child.gameObject;
            }
            else if (child.name == "MostEnergy3Icon")
            {
                PlayerEnergyIcons[2] = child.gameObject;
            }
            else if (child.name == "MostEnergy4Icon")
            {
                PlayerEnergyIcons[3] = child.gameObject;
            }
        }

        updateIcons(PlayerWinIcons, PlayerWins);
        updateIcons(PlayerKillIcons, PlayerKills);
        updateIcons(PlayerEnergyIcons, PlayerEnergy);
        allowInput = true;
    }
}
