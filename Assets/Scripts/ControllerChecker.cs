using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Player1_Jump"))
        {
            Debug.Log("Player1_Jump");
        }
        else if (Input.GetButtonDown("Player2_Jump"))
        {
            Debug.Log("Player2_Jump");
        }
        else if (Input.GetButtonDown("Player3_Jump"))
        {
            Debug.Log("Player3_Jump");
        }
        else if (Input.GetButtonDown("Player4_Jump"))
        {
            Debug.Log("Player4_Jump");
        }
    }
}
