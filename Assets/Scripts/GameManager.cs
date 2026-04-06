using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentNodeName;
    private NodeType currentNodeType = NodeType.None;

    [Header("Player Stats")]
    public float playerHealth = 20f;
    public float playerMaxHealth = 20f;
    private bool playerInputAllowed = true;
    private int currGold = 700;
    public int CurrGold => currGold;

    public bool gameWon = false;

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

    public bool AllowPlayerInput()
    {
        return playerInputAllowed;
    }

    public void SetPlayerInput(bool value)
    {
        playerInputAllowed = value;
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

    public void AddGold(int amount)
    {
        currGold += amount;
        Debug.Log($"Gold added: +{amount} | Total: {currGold}");
        RefreshGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currGold < amount)
        {
            Debug.Log($"Not enough gold. Have {currGold}, need {amount}.");
            return false;
        }
        currGold -= amount;
        Debug.Log($"Gold spent: -{amount} | Total: {currGold}");
        RefreshGoldUI();
        return true;
    }

    public void StealGold(int amount)
    {
        int stolen = Mathf.Min(amount, currGold);
        currGold -= stolen;
        Debug.Log($"Gold stolen: -{stolen} | Total: {currGold}");
        RefreshGoldUI();
    }

    //Reset game state, called when exiting or losing game.
    public void ResetGame()
    {
        currentNodeName = string.Empty;
        currentNodeType = NodeType.None;
        playerHealth = playerMaxHealth;    
    }

    private void RefreshGoldUI()
    {
        if (UIDisplay.Instance != null)
            UIDisplay.Instance.UpdateGoldDisplay();
    }
}
