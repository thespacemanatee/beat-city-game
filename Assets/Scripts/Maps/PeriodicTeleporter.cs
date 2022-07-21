using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicTeleporter : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -50.0f)
            transform.gameObject.SetActive(false); // also sets wallTile (child game obj) to be inactive.
    }
}
