using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Game/MapData")]
public class MapData : ScriptableObject
{
    // Index for assigned map and battle scenes.
    public int mapSceneIndex;       
    public int battleSceneIndex;  

    // List of available enemies in selected map and boss.
    public GameObject[] enemyPrefabs;
    public GameObject bossPrefab;
    public string[] enemyNames;
    public string bossName;
}
