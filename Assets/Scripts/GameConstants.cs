using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects/GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    public int tileCols = 20;
    public int tileRows = 20;
    public float dropRate = 5;
    public float startingRowPos = -10;
    public float startingColPos = -10;
    public float groundLevel;
    public float tilePrefabScale = 1.5f;
    public float gameDurationInSeconds = 60;
}