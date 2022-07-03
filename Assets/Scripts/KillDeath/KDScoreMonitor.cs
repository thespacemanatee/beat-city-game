using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KDScoreMonitor : MonoBehaviour
{
    public IntVariable Player1KillCount;
    public IntVariable Player2KillCount;
    public IntVariable Player3KillCount;
    public IntVariable Player4KillCount;
    public IntVariable Player1DeathCount;
    public IntVariable Player2DeathCount;
    public IntVariable Player3DeathCount;
    public IntVariable Player4DeathCount;

    public Text text;

    public void Start()
    {
        UpdateScore(null, null);
    }

    public void UpdateScore(string killer, string victim){
        if (killer != null && victim != null){
            if (killer != victim){
                UpdateKill(killer);
            }
            UpdateDeath(victim);
        }
        text.text = "Player 1 Kill: " + Player1KillCount.Value.ToString() +
                    "Death: " + Player1DeathCount.Value.ToString() +
                    "\nPlayer 2 Kill: " + Player2KillCount.Value.ToString() +
                    "Death: " + Player2DeathCount.Value.ToString() +
                    "\nPlayer 3 Kill: " + Player3KillCount.Value.ToString() +
                    "Death: " + Player3DeathCount.Value.ToString() +
                    "\nPlayer 4 Kill: " + Player4KillCount.Value.ToString() +
                    "Death: " + Player4DeathCount.Value.ToString();
    }

    void UpdateKill(string killer)
    {
      switch (killer)
      {
        case "Player1":
            Player1KillCount.ApplyChange(1);
            break;
        case "Player2":
            Player2KillCount.ApplyChange(1);
            break;
        case "Player3":
            Player3KillCount.ApplyChange(1);
            break;
        case "Player4":
            Player4KillCount.ApplyChange(1);
            break; 
        default:
            Debug.Log("INCORRECT PLAYER ID");
            break;
      }
         
    }
    void UpdateDeath(string victim)
    {
        switch (victim)
      {
        case "Player1":
            Player1DeathCount.ApplyChange(1);
            break;
        case "Player2":
            Player2DeathCount.ApplyChange(1);
            break;
        case "Player3":
            Player3DeathCount.ApplyChange(1);
            break;
        case "Player4":
            Player4DeathCount.ApplyChange(1);
            break; 
        default:
            Debug.Log("INCORRECT PLAYER ID");
            break;
      }
    }
}
