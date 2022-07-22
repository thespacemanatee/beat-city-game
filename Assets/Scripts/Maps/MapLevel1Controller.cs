using System.Collections.Generic;
using UnityEngine;

internal class TileObject
{
    public bool dropped = false;
    public GameObject gameObject;
    public int index;
    public bool hasTeleporter = false;
    public GameObject teleporter;

    public TileObject(int index, GameObject gameObject)
    {
        this.index = index;
        this.gameObject = gameObject;
    }

    public void setTeleporter(GameObject teleporter)
    {
        this.hasTeleporter = true;
        this.teleporter = teleporter;
    }
}

public class MapLevel1Controller : MonoBehaviour
{
    // public GameConstants gameConstants;
    // public GameObject tilePrefab;
    public CustomDropTileEvent dropTiles;
    // public IntVector3DictVariable positionMap;
    private int _dropRound;
    private int tileRows = 31;
    private int tileCols = 31;
    private readonly List<TileObject> _tiles = new();

    private void Awake()
    {
        // for (var r = 0; r < gameConstants.tileRows; r++)
        // for (var c = 0; c < gameConstants.tileCols; c++)
        // {
        //     var tile = Instantiate(tilePrefab);
        //     tile.GetComponent<TileController>().index = index;
        //     tile.transform.position = new Vector3((r + gameConstants.startingRowPos) * gameConstants.tilePrefabScale,
        //         gameConstants.groundLevel, (c + gameConstants.startingColPos) * gameConstants.tilePrefabScale);
        //     _tiles.Add(new TileObject(index, gameObject));
        //     index++;
        // }
        // positionMap.Reset();

        var index = 0;
        foreach (Transform col in transform)
        {
            foreach (Transform tile in col)
            {
                tile.GetComponent<TileController>().index = index; // set index for tile
                _tiles.Add(new TileObject(index, tile.gameObject)); // add it to list of tiles managed by map
                // positionMap.AddItem(index, tile.position);
                index++;
            }
            // assumes same number of tiles and rows
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void ShrinkMap()
    {
        // Drop 2 side columns
        var remainingSideLength = tileRows - 2 * _dropRound;
        if (remainingSideLength > 3)
        {
            var dropIndex = new List<int>();
            var startIndex = _dropRound * tileRows + _dropRound;
            for (var i = startIndex; i < startIndex + remainingSideLength; i++)
            {
                dropIndex.Add(i);
                // Drops opposite column -- only works assuming our map is a square, otherwise calculate the other column separately
                dropIndex.Add(i + (remainingSideLength - 1) * tileCols);
                // positionMap.RemoveItem(i);
                // positionMap.RemoveItem(i + (remainingSideLength - 1) * tileCols);
            }

            // Drop 2 side rows
            for (var i = startIndex + tileRows;
                 i < startIndex + (remainingSideLength - 1) * tileCols;
                 i += tileRows)
            {
                dropIndex.Add(i);
                dropIndex.Add(i + (remainingSideLength - 1));
                // positionMap.RemoveItem(i);
                // positionMap.RemoveItem(i + (remainingSideLength - 1));
            }

            _dropRound++;
            // Debug.Log(string.Join(",", dropIndex));
            // Debug.Log(dropIndex.Count);
            dropTiles.Invoke(dropIndex);
        }
    }
}