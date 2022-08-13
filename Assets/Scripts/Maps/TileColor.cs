using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColor : MonoBehaviour
{
    public Material newMaterial;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            foreach(Transform childChild in child.transform) 
            {
                childChild.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
