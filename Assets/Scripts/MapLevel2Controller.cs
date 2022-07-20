using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class PlatformObject
{
    public int index;
    public int smallestTileIndex;
    public GameObject gameObject;
    public List<TileObject> tiles;

    public PlatformObject(int index, int smallestTileIndex, GameObject gameObject)
    {
        this.index = index;
        this.smallestTileIndex = smallestTileIndex;
        this.gameObject = gameObject;
    }

    public void setTiles(List<TileObject> tiles)
    {
        this.tiles = tiles;
    }
}


public class MapLevel2Controller : MonoBehaviour
{
    public CustomDropTileEvent dropTiles;
    private List<PlatformObject> _platforms = new List<PlatformObject>();
    private int _dropRound;
    public int rows = 11;
    public int cols = 11;

    void Awake()
    {
        var tileIndex = 0;
        var platformIndex = 0;
        foreach (Transform platform in transform)
        {
            _platforms.Add(new PlatformObject(platformIndex, tileIndex, platform.gameObject));
            List<TileObject> tiles = new List<TileObject>();
            foreach (Transform col in platform)
            {
                foreach (Transform tile in col)
                {
                    tile.GetComponent<TileController>().index = tileIndex; // set index for tile
                    tiles.Add(new TileObject(tileIndex, tile.gameObject)); // add it to list of tiles managed by map
                    tileIndex++;
                }
            }
            _platforms[platformIndex].setTiles(tiles);
            platformIndex++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShrinkMap()
    {
        var remainingSideLength = rows - _dropRound;
        var dropIndex = new List<int>();

        if (remainingSideLength > 4)
        {
            foreach (PlatformObject platform in _platforms)
            {
                bool useFirstCol = platform.gameObject.transform.position.x < 0;
                bool useFirstRow = platform.gameObject.transform.position.z < 0;

                int colStartIndex = useFirstCol ? _dropRound * rows : (cols - _dropRound - 1) * rows;
                for (var i = colStartIndex; i < colStartIndex + rows; i++)
                {
                    bool shouldAdd = (useFirstRow && i >= colStartIndex + _dropRound) || (!useFirstRow && i < colStartIndex + remainingSideLength);
                    if (shouldAdd)
                    {
                        dropIndex.Add(i + platform.smallestTileIndex);
                    }
                }
                int rowStartIndex = useFirstRow ? colStartIndex + _dropRound : colStartIndex + remainingSideLength - 1;
                for (var i = useFirstCol ? rowStartIndex + rows : rowStartIndex - rows; i < rows * cols && i >= 0;)
                {
                    dropIndex.Add(i + platform.smallestTileIndex);
                    i += useFirstCol ? rows : -rows;
                }
            }
            // Debug.Log(string.Join(",", dropIndex));
            // Debug.Log(dropIndex.Count);

            _dropRound++;
            dropTiles.Invoke(dropIndex);
        }
    }
}
