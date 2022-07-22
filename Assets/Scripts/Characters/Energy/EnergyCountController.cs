using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyCountController : MonoBehaviour
{
    public IntVariable Player1EnergyCount;
    public IntVariable Player2EnergyCount;
    public IntVariable Player3EnergyCount;
    public IntVariable Player4EnergyCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.name == "MinimalCharacter") Player1EnergyCount.ApplyChange(1);
            else if (other.name == "MinimalCharacter 1") Player2EnergyCount.ApplyChange(1);
            else if (other.name == "MinimalCharacter 2") Player3EnergyCount.ApplyChange(1);
            else if (other.name == "MinimalCharacter 3") Player4EnergyCount.ApplyChange(1);
            
        }
        
    }
}
