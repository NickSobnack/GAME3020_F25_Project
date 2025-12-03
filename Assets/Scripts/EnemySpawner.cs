using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    // Enemy spawner that randomly spawns a set number of enemies at predefined spawn points with random names.
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; 
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private GameObject bossPrefab; 
    [SerializeField] private string[] enemyNames = { "Clara", "Gloria", "Teadon", "Selena", "Auguste", "Simona", "Parsifal"};
    [SerializeField] private string bossName = "Camelot";
    [SerializeField] private int maxEnemiesToSpawn = 3;

    [SerializeField] private GameObject bossPopupPanel;
    [SerializeField] private Button continueButton;

    private void Start()
    {
        NodeType nodeType = GameManager.Instance.GetCurrentNodeType();
        Debug.Log("Spawner starting, node type = " + nodeType);

        if (nodeType == NodeType.Enemy)
            SpawnEnemies();
        else if (nodeType == NodeType.Boss)
            SpawnBoss();
    }

    public void Spawn(NodeType nodeType)
    {
        if (nodeType == NodeType.Enemy)
        {
            SpawnEnemies();
        }
        else if (nodeType == NodeType.Boss)
        {
            SpawnBoss();
        }
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

    // Spawn boss at first spawn point with one static name.
    private void SpawnBoss()
    {
        if (bossPrefab != null && spawnPoints.Length > 0)
        {
            Vector3 offset = new Vector3(1, 0.5f, 0);
            GameObject bossInstance = Instantiate(bossPrefab, spawnPoints[0].position + offset, Quaternion.identity);

            EnemyBase enemyBase = bossInstance.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.enemyName = bossName;
                bossInstance.name = bossName;
            }
        }
        ShowBossPopup();
    }

    private void ShowBossPopup()
    {
        if (bossPopupPanel != null)
        {
            bossPopupPanel.SetActive(true);
            Time.timeScale = 0f; 

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                bossPopupPanel.SetActive(false);
                Time.timeScale = 1f; 
            });
        }
    }
}

