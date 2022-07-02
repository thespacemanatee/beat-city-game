using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class TileObject {
    public int index;
    public GameObject gameObject;
    public bool dropped = false;

    public TileObject(int index, GameObject gameObject) {
        this.index = index;
        this.gameObject = gameObject;
    }
}

public class GroundController : MonoBehaviour
{
    public GameConstants gameConstants;
    public GameObject tilePrefab;
    public CustomDropTileEvent dropTiles;
    private List<TileObject> tiles = new List<TileObject>();
    private int dropRound = 0;

    void Awake() {
        int index = 0;
        for (int r=0; r<gameConstants.tileRows; r++) {
            for (int c=0; c<gameConstants.tileCols; c++) {
                GameObject tile = (GameObject)Instantiate(tilePrefab);
                tile.GetComponent<TileController>().index = index;
                tile.transform.position = new Vector3((r+gameConstants.startingRowPos)*gameConstants.tilePrefabScale, gameConstants.groundLevel, (c+gameConstants.startingColPos)*gameConstants.tilePrefabScale);
                tiles.Add(new TileObject(index, gameObject));
                index++;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dropRound == 0) {
            // Drop 2 side columns
            int remainingSideLength = gameConstants.tileRows-(2*dropRound);
            if (remainingSideLength > 2) {
                List<int> dropIndex = new List<int>();
                int startIndex = (dropRound * gameConstants.tileRows) + dropRound;
                for (int i=startIndex; i<startIndex+remainingSideLength; i++) {
                    dropIndex.Add(i);
                    // Drops opposite column -- only works assuming our map is a square, otherwise calculate the other column separately
                    dropIndex.Add(i+(remainingSideLength-1)*gameConstants.tileCols);
                }

                // Drop 2 side rows
                for (int i=startIndex+gameConstants.tileRows; i<startIndex+(remainingSideLength-1)*gameConstants.tileCols; i+=gameConstants.tileRows) {
                    dropIndex.Add(i);
                    dropIndex.Add(i+(remainingSideLength-1));
                }
                dropRound++;
                dropTiles.Invoke(dropIndex);
            }
        }
    }
}
