using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    public int tileCols = 2;
    public int tileRows = 2;
    public float dropRate = 5;
    public float startingRowPos = -10;
    public float startingColPos = -10;
    public float groundLevel = 0;
    public float tilePrefabScale = 1.5f;

}
