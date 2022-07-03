using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public int index;

    private Rigidbody _tileRigidbody;

    // Start is called before the first frame update
    private void Start()
    {
        _tileRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DropTile(List<int> indexes)
    {
        for (var i = 0; i < indexes.Count; i++)
            if (indexes[i] == index)
                _tileRigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }
}