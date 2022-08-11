using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public GameObject forcefield;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Forcefield is " + forcefield);
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(5f);
        forcefield.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attack")
        {
            forcefield.SetActive(false);
        }
    }
    
}
