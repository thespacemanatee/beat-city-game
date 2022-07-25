using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    // Manually assign all Game Objects under the UI components 
    GameObject WaitText;

    GameObject Player1ReadyIcon;
    GameObject Player2ReadyIcon;
    GameObject Player3ReadyIcon;
    GameObject Player4ReadyIcon;    

    bool player1ReadyState = false;
    bool player2ReadyState = false;
    bool player3ReadyState = false;
    bool player4ReadyState = false;

    bool isCoroutineReady = true;

    public Sprite Player1NotReady;
    public Sprite Player2NotReady;
    public Sprite Player3NotReady;
    public Sprite Player4NotReady;

    public Sprite Player1Ready;
    public Sprite Player2Ready;
    public Sprite Player3Ready;
    public Sprite Player4Ready;

    public GameObject inventoryUI;

    void Awake()
    {
        Time.timeScale = 0.0f; // pause game initially
        foreach (Transform eachChild in transform)
        {
            if (eachChild.name == "Player1ReadyIcon")
            {
                Player1ReadyIcon = eachChild.gameObject;
            }
            else if (eachChild.name == "Player2ReadyIcon")
            {
                Player2ReadyIcon = eachChild.gameObject;
            }
            else if (eachChild.name == "Player3ReadyIcon")
            {
                Player3ReadyIcon = eachChild.gameObject;
            }
            else if (eachChild.name == "Player4ReadyIcon")
            {
                Player4ReadyIcon = eachChild.gameObject;
            }
            else if (eachChild.name == "WaitText")
            {
                WaitText = eachChild.gameObject;
                WaitText.GetComponent<Text>().text = "Waiting for players ... \n Press 'Jump' to ready up";
            }
            Debug.Log(eachChild.name);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // constant checking for inputs 
        if (Input.GetButtonDown("Player1_Jump"))
        {
            if (!player1ReadyState)
            {
                Player1ReadyIcon.GetComponent<Image>().sprite = Player1Ready;
                Debug.Log(Player1ReadyIcon.GetComponent<Image>().sprite);
                player1ReadyState = true;
            }
            else
            {
                Player1ReadyIcon.GetComponent<Image>().sprite = Player1NotReady;
                Debug.Log(Player1ReadyIcon.GetComponent<Image>().sprite);
                player1ReadyState = false;
            }
            
        }
        else if (Input.GetButtonDown("Player2_Jump"))
        {
            if (!player2ReadyState)
            {
                Player2ReadyIcon.GetComponent<Image>().sprite = Player2Ready;
                Debug.Log(Player2ReadyIcon.GetComponent<Image>().sprite);
                player2ReadyState = true;
            }
            else
            {
                Player2ReadyIcon.GetComponent<Image>().sprite = Player2NotReady;
                Debug.Log(Player2ReadyIcon.GetComponent<Image>().sprite);
                player2ReadyState = false;
            }
        }
        else if (Input.GetButtonDown("Player3_Jump"))
        {
            if (!player3ReadyState)
            {
                Player3ReadyIcon.GetComponent<Image>().sprite = Player3Ready;
                Debug.Log(Player3ReadyIcon.GetComponent<Image>().sprite);
                player3ReadyState = true;
            }
            else
            {
                Player3ReadyIcon.GetComponent<Image>().sprite = Player3NotReady;
                Debug.Log(Player3ReadyIcon.GetComponent<Image>().sprite);
                player3ReadyState = false;
            }
        }
        else if (Input.GetButtonDown("Player4_Jump"))
        {
            if (!player4ReadyState)
            {
                Player4ReadyIcon.GetComponent<Image>().sprite = Player4Ready;
                Debug.Log(Player4ReadyIcon.GetComponent<Image>().sprite);
                player4ReadyState = true;
            }
            else
            {
                Player4ReadyIcon.GetComponent<Image>().sprite = Player4NotReady;
                Debug.Log(Player4ReadyIcon.GetComponent<Image>().sprite);
                player4ReadyState = false;
            }
        }
        if (player1ReadyState && player2ReadyState && player3ReadyState && player4ReadyState && isCoroutineReady)
        {
            isCoroutineReady = false;
            StartCoroutine(StartCountdown());
        }
    }

    IEnumerator StartCountdown()
    {
        WaitForSecondsRT wait = new WaitForSecondsRT(1);
        int counter = 3;
        WaitText.GetComponent<Text>().text = $"Players ready, game starting in \n {counter}";
        while (counter > 0){
            // if not then wait for the remaining number of seconds while updating the message
            yield return wait.NewTime(1);
            counter --;
            WaitText.GetComponent<Text>().text = $"Players ready, game starting in \n {counter}";
        }
        WaitText.GetComponent<Text>().text = $"Players ready, game starting in \n {counter}";
        yield return wait.NewTime(1);
        Time.timeScale = 1.0f; // start game after 
        this.gameObject.SetActive(false);
        inventoryUI.SetActive(true);
        isCoroutineReady = true;
    }
}

public class WaitForSecondsRT : CustomYieldInstruction
{
    float m_Time;
    public override bool keepWaiting
    {
        get { return (m_Time -= Time.unscaledDeltaTime) > 0;}
    }
    public WaitForSecondsRT(float aWaitTime)
    {
        m_Time = aWaitTime;
    }
    public WaitForSecondsRT NewTime(float aTime)
    {
        m_Time = aTime;
        return this;
    }
}
