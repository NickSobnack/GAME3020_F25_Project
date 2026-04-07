using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string currentNodeName;
    private NodeType currentNodeType = NodeType.None;

    [Header("Player Stats")]
    public float playerHealth = 20f;
    public float playerMaxHealth = 20f;
    private bool playerInputAllowed = true;
    public bool gameWon = false;

    [Header("Gold & Shop")]
    private int currGold = 700;
    public int CurrGold => currGold; 
    private HashSet<string> purchasedItems = new();
    public int goldBonusNextFight = 0; 
    public bool HasPurchased(string id) => purchasedItems.Contains(id);

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
        RefreshGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (currGold < amount) return false;

        currGold -= amount;
        RefreshGoldUI();
        return true;
    }

    // Marks an item as purchased to prevent one time items from being bought again and to track item bought.
    public void MarkPurchased(string id)
    {
        purchasedItems.Add(id);
    }

    public void StealGold(int amount)
    {
        int stolen = Mathf.Min(amount, currGold);
        currGold -= stolen;
        RefreshGoldUI();
    }

    //Reset game state, called when exiting or losing game.
    public void ResetGame()
    {
        currentNodeName = string.Empty;
        currentNodeType = NodeType.None;
        playerHealth = playerMaxHealth;
        purchasedItems.Clear();
        goldBonusNextFight = 0;
    }

    private void RefreshGoldUI()
    {
        if (UIDisplay.Instance != null)
            UIDisplay.Instance.UpdateGoldDisplay();
    }
}
