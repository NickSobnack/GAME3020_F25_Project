using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class EnemySpawner : MonoBehaviour
    {

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxEnemiesToSpawn = 3;
    [SerializeField] private PlayableDirector bossTimeline;
    [SerializeField] private GameObject bossPopupPanel;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private Button continueButton;

    private GameObject bossInstance;
    private MapData mapData;

    private void Start()
    {
        mapData = GameManager.Instance.CurrentMap;
        NodeType nodeType = GameManager.Instance.GetCurrentNodeType();
        if (nodeType == NodeType.Enemy)
            SpawnEnemies();
        else if (nodeType == NodeType.Boss)
            SpawnBoss();
    }

    private void SpawnEnemies()
    {
        int count = Mathf.Min(maxEnemiesToSpawn, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = mapData.enemyPrefabs[Random.Range(0, mapData.enemyPrefabs.Length)];
            GameObject instance = Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);
            string randomName = mapData.enemyNames[Random.Range(0, mapData.enemyNames.Length)];
            EnemyBase enemyBase = instance.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                enemyBase.enemyName = randomName;
                instance.name = randomName;
            }
        }
    }

    private void SpawnBoss()
    {
        PlayBossCutscene();
        if (mapData.bossPrefab != null && spawnPoints.Length > 0)
        {
            Vector3 offset = new Vector3(1, 0.5f, 0);
            bossInstance = Instantiate(mapData.bossPrefab, spawnPoints[0].position + offset, Quaternion.identity);
            EnemyBase enemyBase = bossInstance.GetComponent<EnemyBase>();

            if (enemyBase != null)
            {
                enemyBase.enemyName = mapData.bossName;
                bossInstance.name = mapData.bossName;
            }

            bossInstance.gameObject.SetActive(false);
        }
    }

    private void PlayBossCutscene()
    {
        GameManager.Instance.SetPlayerInput(false);
        Time.timeScale = 0f; 
        if (gameplayUI != null)
            gameplayUI.SetActive(false);
        bossTimeline.gameObject.SetActive(true);
        bossTimeline.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
        bossTimeline.Play();
        bossTimeline.stopped += OnBossTimelineFinished;
    }

    private void OnBossTimelineFinished(PlayableDirector director)
    {
        bossTimeline.stopped -= OnBossTimelineFinished;
        bossTimeline.gameObject.SetActive(false); 
        ShowBossPopup();
    }

    private void ShowBossPopup()
    {
        if (bossPopupPanel != null)
        {
            bossPopupPanel.SetActive(true);

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                bossPopupPanel.SetActive(false);
                if (gameplayUI != null)
                    gameplayUI.SetActive(true);
                bossInstance.gameObject.SetActive(true);
                Time.timeScale = 1f; 
                GameManager.Instance.SetPlayerInput(true);
            });
        }
    }
}

