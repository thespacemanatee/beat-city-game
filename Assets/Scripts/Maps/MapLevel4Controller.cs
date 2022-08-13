using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel4Controller : MonoBehaviour
{
    public IntVector3DictVariable positionMap;
    private List<TileObject> _tiles = new List<TileObject>();

    void Awake()
    {
        positionMap.Reset();

        var index = 0;
        foreach (Transform col in transform)
        {
            foreach (Transform tile in col)
            {
                tile.GetComponent<TileController>().index = index; // set index for tile
                _tiles.Add(new TileObject(index, tile.gameObject)); // add it to list of tiles managed by map
                positionMap.AddItem(index, tile.position);
                index++;
            }
            // assumes same number of tiles and rows
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
