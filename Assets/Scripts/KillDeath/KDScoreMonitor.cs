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

    public void UpdateScore(string killer, string victim)
    {
        if (killer != null && victim != null)
        {
            if (killer != victim) UpdateKill(killer);
            UpdateDeath(victim);
        }

        text.text = "Player 1 Kill: " + Player1KillCount.Value +
                    "Death: " + Player1DeathCount.Value +
                    "\nPlayer 2 Kill: " + Player2KillCount.Value +
                    "Death: " + Player2DeathCount.Value +
                    "\nPlayer 3 Kill: " + Player3KillCount.Value +
                    "Death: " + Player3DeathCount.Value +
                    "\nPlayer 4 Kill: " + Player4KillCount.Value +
                    "Death: " + Player4DeathCount.Value;
    }

    public void ResetScore()
    {
        Player1KillCount.SetValue(0);
        Player2KillCount.SetValue(0);
        Player3KillCount.SetValue(0);
        Player4KillCount.SetValue(0);
        Player1DeathCount.SetValue(0);
        Player2DeathCount.SetValue(0);
        Player3DeathCount.SetValue(0);
        Player4DeathCount.SetValue(0);
    }

    private void UpdateKill(string killer)
    {
        switch (killer)
        {
            case "Player1":
                Debug.Log("Player 1 Kill Count +1, current value is " + Player1KillCount.Value);
                Player1KillCount.ApplyChange(1);
                break;
            case "Player2":
                Debug.Log("Player 2 Kill Count +1, current value is " + Player2KillCount.Value);
                Player2KillCount.ApplyChange(1);
                break;
            case "Player3":
                Debug.Log("Player 3 Kill Count +1, current value is " + Player3KillCount.Value);
                Player3KillCount.ApplyChange(1);
                break;
            case "Player4":
                Debug.Log("Player 4 Kill Count +1, current value is " + Player4KillCount.Value);
                Player4KillCount.ApplyChange(1);
                break;
            default:
                Debug.Log("INCORRECT PLAYER ID");
                break;
        }
    }

    private void UpdateDeath(string victim)
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