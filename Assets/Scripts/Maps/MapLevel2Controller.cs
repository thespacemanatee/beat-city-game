using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

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
    public IntVector3DictVariable positionMap;
    public Material teleporterActiveTileMaterial;
    public Material teleporterInactiveTileMaterial;
    private List<PlatformObject> _platforms = new List<PlatformObject>();
    private int _dropRound;
    public int rows = 15;
    public int cols = 15;
    private float teleporterAppearChance = 0.3f;
    private float teleporterAppearDuration = 5f;
    private float teleporterAppearInterval = 5.5f;
    private int minRemainingSideLength = 6;

    void Awake()
    {
        positionMap.Reset();

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
                    TileObject tileObject = new TileObject(tileIndex, tile.gameObject);
                    foreach (Transform child in tile)
                    {
                        if (child.CompareTag("Teleporter"))
                        {
                            tileObject.setTeleporter(child.gameObject);
                            child.gameObject.SetActive(false);
                        }
                    }
                    tile.GetComponent<TileController>().index = tileIndex; // set index for tile
                    tiles.Add(tileObject); // add it to list of tiles managed by map
                    positionMap.AddItem(tileIndex, tile.position);
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
        InvokeRepeating("RandomizeTeleporterAppearance", 1, teleporterAppearInterval);
    }

    void RandomizeTeleporterAppearance()
    {
        foreach (TileObject tileObject in _platforms[0].tiles)
        {
            // Make sure tile has teleporter and randomizer decides teleporter should appear and teleporter is not already active
            if (tileObject.hasTeleporter && Random.value > (1 - teleporterAppearChance) && !tileObject.teleporter.activeSelf)
            {
                // Get chain of teleporters and set all to active
                List<GameObject> teleporters = new List<GameObject>();
                teleporters.Add(tileObject.teleporter);
                setTeleporterAppear(tileObject.teleporter);
                GameObject curTeleporter = tileObject.teleporter.GetComponent<Teleporter>().Destination.gameObject;
                int count = 0;
                while (curTeleporter != tileObject.teleporter && count < 10)
                {
                    teleporters.Add(curTeleporter);
                    setTeleporterAppear(curTeleporter);
                    curTeleporter = curTeleporter.GetComponent<Teleporter>().Destination.gameObject;
                    count++;
                }
                Debug.Log("Count: " + count);
                StartCoroutine(SetTeleporterInactive(teleporters));
            }
        }
    }

    IEnumerator SetTeleporterInactive(List<GameObject> teleporters)
    {
        yield return new WaitForSeconds(teleporterAppearDuration);
        foreach (GameObject teleporter in teleporters)
        {
            if (teleporter != null)
            {
                teleporter.SetActive(false);
                MeshRenderer renderer = teleporter.transform.parent.gameObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.material = teleporterInactiveTileMaterial;
                }
            }
        }
    }

    private void setTeleporterAppear(GameObject teleporter)
    {
        if (teleporter != null)
        {
            teleporter.SetActive(true);
            MeshRenderer renderer = teleporter.transform.parent.gameObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = teleporterActiveTileMaterial;
            }
        }
    }

    public void ShrinkMap()
    {
        var remainingSideLength = rows - _dropRound;
        var dropIndex = new List<int>();

        if (remainingSideLength > minRemainingSideLength)
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
                        positionMap.RemoveItem(i + platform.smallestTileIndex);
                    }
                }
                int rowStartIndex = useFirstRow ? colStartIndex + _dropRound : colStartIndex + remainingSideLength - 1;
                for (var i = useFirstCol ? rowStartIndex + rows : rowStartIndex - rows; i < rows * cols && i >= 0;)
                {
                    dropIndex.Add(i + platform.smallestTileIndex);
                    positionMap.RemoveItem(i + platform.smallestTileIndex);
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
