using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopLogic : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text[] itemTexts;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private string[] commonItems = new string[]
    { "Apple", "Pear", "Lettuce",};

    private string[] uncommonItems = new string[]
    {"Meat", "Cheese", "Milk"};

    private string[] rareItems = new string[]
    {"Cookie", "Mako", "Pizza"};

    private int commonCost = 5;
    private int uncommonCost = 10;
    private int rareCost = 20;

    private string[] chosenItems = new string[3];
    private int[] chosenCosts = new int[3];

    private int selectedIndex = -1;

    void Start()
    {
        confirmButton.interactable = false;
        cancelButton.interactable = false;

        RefreshGoldDisplay();
        RandomizeItems();
    }

    void RefreshGoldDisplay()
    {
        goldText.text = $"Gold: {GameManager.Instance.CurrGold}";
    }

    void RandomizeItems()
    {
        for (int i = 0; i < 3; i++)
        {
            PickRandomItem(i);
            itemTexts[i].text = $"{chosenItems[i]} - {chosenCosts[i]}g";
        }
    }

    void PickRandomItem(int slot)
    {
        int roll = Random.Range(0, 100);

        if (roll < 70)
        {
            int index = Random.Range(0, commonItems.Length);
            chosenItems[slot] = commonItems[index];
            chosenCosts[slot] = commonCost;
        }
        else if (roll < 90)
        {
            int index = Random.Range(0, uncommonItems.Length);
            chosenItems[slot] = uncommonItems[index];
            chosenCosts[slot] = uncommonCost;
        }
        else
        {
            int index = Random.Range(0, rareItems.Length);
            chosenItems[slot] = rareItems[index];
            chosenCosts[slot] = rareCost;
        }
    }

    public void SelectItem(int index)
    {
        selectedIndex = index;
        confirmButton.interactable = true;
        cancelButton.interactable = true;

        for (int i = 0; i < itemTexts.Length; i++)
            itemTexts[i].color = Color.black;

        itemTexts[index].color = Color.green;
    }

    public void ConfirmPurchase()
    {
        if (selectedIndex == -1)
            return;

        string name = chosenItems[selectedIndex];
        int cost = chosenCosts[selectedIndex];

        if (GameManager.Instance.SpendGold(cost))
        {
            RefreshGoldDisplay();
            CloseShop();
        }
        else
        {
            Debug.Log("Not enough gold.");
        }

        ResetSelection();
    }

    public void CancelSelection()
    {
        ResetSelection();
    }

    private void ResetSelection()
    {
        selectedIndex = -1;

        for (int i = 0; i < itemTexts.Length; i++)
            itemTexts[i].color = Color.black;

        confirmButton.interactable = false;
        cancelButton.interactable = false;
    }

    public void CloseShop()
    {
        BackgroundBlurManager.Instance.RegisterPanelClosed();
        gameObject.SetActive(false);
    }

    public void ResetShop()
    {
        selectedIndex = -1;

        confirmButton.interactable = false;
        cancelButton.interactable = false;

        for (int i = 0; i < itemTexts.Length; i++)
            itemTexts[i].color = Color.black;

        RandomizeItems();

        RefreshGoldDisplay();

        Debug.Log("Shop reset.");
    }

}
