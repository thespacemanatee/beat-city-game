using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDController : MonoBehaviour
{
    public IntVariable killCount;
    public IntVariable deathCount;
    // Start is called before the first frame update
    void Start()
    {
        killCount.SetValue(0);
        deathCount.SetValue(0);
    }

}
