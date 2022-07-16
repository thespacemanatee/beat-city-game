using System.Collections.Generic;
using UnityEngine;

internal class TileObject
{
    public bool dropped = false;
    public GameObject gameObject;
    public int index;

    public TileObject(int index, GameObject gameObject)
    {
        this.index = index;
        this.gameObject = gameObject;
    }
}

public class GroundController : MonoBehaviour
{
    public GameConstants gameConstants;
    public GameObject tilePrefab;
    public CustomDropTileEvent dropTiles;
    private readonly List<TileObject> _tiles = new();
    private int _dropRound;

    private void Awake()
    {
        var index = 0;
        for (var r = 0; r < gameConstants.tileRows; r++)
        for (var c = 0; c < gameConstants.tileCols; c++)
        {
            var tile = Instantiate(tilePrefab);
            tile.GetComponent<TileController>().index = index;
            tile.transform.position = new Vector3((r + gameConstants.startingRowPos) * gameConstants.tilePrefabScale,
                gameConstants.groundLevel, (c + gameConstants.startingColPos) * gameConstants.tilePrefabScale);
            _tiles.Add(new TileObject(index, gameObject));
            index++;
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
        var remainingSideLength = gameConstants.tileRows - 2 * _dropRound;
        if (remainingSideLength > 3)
        {
            var dropIndex = new List<int>();
            var startIndex = _dropRound * gameConstants.tileRows + _dropRound;
            for (var i = startIndex; i < startIndex + remainingSideLength; i++)
            {
                dropIndex.Add(i);
                // Drops opposite column -- only works assuming our map is a square, otherwise calculate the other column separately
                dropIndex.Add(i + (remainingSideLength - 1) * gameConstants.tileCols);
            }

            // Drop 2 side rows
            for (var i = startIndex + gameConstants.tileRows;
                 i < startIndex + (remainingSideLength - 1) * gameConstants.tileCols;
                 i += gameConstants.tileRows)
            {
                dropIndex.Add(i);
                dropIndex.Add(i + (remainingSideLength - 1));
            }

            _dropRound++;
            dropTiles.Invoke(dropIndex);
        }
    }
}