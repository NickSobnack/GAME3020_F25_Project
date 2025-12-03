using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentNodeName;
    private NodeType currentNodeType = NodeType.SafeZone;

    [Header("Player Stats")]
    public float playerHealth = 20f;
    public float playerMaxHealth = 20f;
    public int playerGold = 0;

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

    // Sets and gets the current node info.
    public void SetCurrentNode(Node node)
    {
        currentNodeName = node.name;
        currentNodeType = node.nodeType;
    }

    public string GetCurrentNodeName()
    {
        return currentNodeName;
    }

    public NodeType GetCurrentNodeType()
    {
        return currentNodeType;
    }

    // Load scene by index.
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Delay coroutine in between loading scenes.
    public void DelayLoadScene(int sceneIndex, float delay)
    {
        StartCoroutine(DelayScene(sceneIndex, delay));
    }

    private IEnumerator DelayScene(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneIndex);
    }

    public void GetGold(int amount)
    {
        playerGold += amount;
    }

    public void SpendGold(int amount)
    {
        playerGold = Mathf.Max(0, playerGold - amount);
    }

    //Reset game state, called when exiting or losing game.
    public void ResetGame()
    {
        currentNodeName = string.Empty;
        currentNodeType = NodeType.SafeZone;

        playerHealth = playerMaxHealth;
        playerGold = 0;
    }
}
