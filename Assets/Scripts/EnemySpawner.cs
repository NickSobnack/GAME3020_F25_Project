using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Enemy spawner that randomly spawns a set number of enemies at predefined spawn points with random names.
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; 
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private string[] enemyNames = { "Clara", "Gloria", "Teadon", "Selena", "Auguste", "Simona", "Camelot", "Parsifal" };

    [SerializeField] private int maxEnemiesToSpawn = 3;

    private void Start()
    {
        SpawnEnemies();
    }

    // Spawns a random number of enemies at random spawn points with random names.
    private void SpawnEnemies()
    {
        int enemiesToSpawn = Mathf.Min(maxEnemiesToSpawn, spawnPoints.Length);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

            string randomName = enemyNames[Random.Range(0, enemyNames.Length)];

            EnemyBase enemyBase = enemyInstance.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.enemyName = randomName;
                enemyInstance.name = randomName;
            }
        }
    }

}
