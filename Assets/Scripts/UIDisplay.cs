using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIDisplay : MonoBehaviour
{
    public static UIDisplay Instance { get; private set; }

    [SerializeField] private GameObject uiPanel; 
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private int[] hiddenSceneIndexes = { 0 }; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        UpdateGoldDisplay();
        CheckSceneVisibility(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateGoldDisplay();
        CheckSceneVisibility(scene.buildIndex);
    }

    public void UpdateGoldDisplay()
    {
        if (uiPanel != null && GameManager.Instance != null)
            goldText.text = GameManager.Instance.CurrGold.ToString();
    }

    private void CheckSceneVisibility(int sceneIndex)
    {
        bool shouldHide = System.Array.IndexOf(hiddenSceneIndexes, sceneIndex) >= 0;
        if (uiPanel != null) uiPanel.gameObject.SetActive(!shouldHide);
    }
}