using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryMenuController : MonoBehaviour
{
    public IntVariable Player1Win;
    public IntVariable Player2Win;
    public IntVariable Player3Win;
    public IntVariable Player4Win;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Player 1 Win" + Player1Win.Value.ToString());
        
        Debug.Log("Player 2 Win" + Player2Win.Value.ToString());
        
        Debug.Log("Player 3 Win" + Player3Win.Value.ToString());
        
        Debug.Log("Player 4 Win" + Player4Win.Value.ToString());
    }
}
