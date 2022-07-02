using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int index;
    private Rigidbody tileRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        tileRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void dropTile(List<int> indexes) {
        for (var i = 0; i < indexes.Count; i++) {
            if (indexes[i] == index) {
                tileRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
            }
        }
    }
}
