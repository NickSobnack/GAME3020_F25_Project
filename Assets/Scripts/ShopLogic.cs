using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopLogic : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private ShopItemSlot[] itemSlots;

    [SerializeField] private List<ItemData> commonItems;
    [SerializeField] private List<ItemData> uncommonItems;
    [SerializeField] private List<ItemData> rareItems;

    private List<ItemData> chosenItems = new();

    private Animator shopkeeperAnim;
    private int selectedIndex = -1;

    void Awake()
    {
        shopkeeperAnim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        if (itemSlots == null || itemSlots.Length == 0)
            itemSlots = GetComponentsInChildren<ShopItemSlot>(true);

        for (int i = 0; i < itemSlots.Length; i++)
        {
            int idx = i;
            itemSlots[i].ItemButton.onClick.RemoveAllListeners();
            itemSlots[i].ItemButton.onClick.AddListener(() => SelectItem(idx));
        }

        shopkeeperAnim.Play("ShopkeeperShop");
    }

    void Start()
    {
        confirmButton.interactable = false;
        cancelButton.interactable = false;

        RandomizeShopItems();
    }


    // Randomly picks items at the start of each shop visit and populates the UI.
    void RandomizeShopItems()
    {
        chosenItems.Clear();

        for (int i = 0; i < 3; i++)
        {
            ItemData item = PickRandomItem();
            chosenItems.Add(item);

            var slot = itemSlots[i];

            slot.ItemName.text = $"{item.itemName}";
            slot.ItemName.color = item.rarityColor;   
            slot.ItemIcon.sprite = item.icon;
            slot.ItemBorder.gameObject.SetActive(false);
        }
    }

    ItemData PickRandomItem()
    {
        int roll = Random.Range(0, 100);
        List<ItemData> pool;

        if (roll < 70) pool = commonItems;
        else if (roll < 90) pool = uncommonItems;
        else pool = rareItems;

        ItemData item = pool[Random.Range(0, pool.Count)];

        if (item.isOneTimePurchase && GameManager.Instance.HasPurchased(item.id))
            return PickRandomItem();

        return item;
    }

    public void SelectItem(int index)
    {
        selectedIndex = index;
        confirmButton.interactable = true;
        cancelButton.interactable = true;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].ItemBorder.gameObject.SetActive(false);
            itemSlots[i].ItemName.color = chosenItems[i].rarityColor;
        }

        itemSlots[index].ItemBorder.gameObject.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        if (selectedIndex == -1)
            return;

        ItemData item = chosenItems[selectedIndex];

        if (GameManager.Instance.SpendGold(item.cost))
        {
            ApplyItemEffect(item);

            if (item.isOneTimePurchase)
                GameManager.Instance.MarkPurchased(item.id);

            CloseShop();
        }
        else
        {
            Debug.Log("Not enough gold.");
        }

        ResetSelection();
    }
 
    void ApplyItemEffect(ItemData item)
    {
        switch (item.effectType)
        {
            case ItemEffect.GoldBoost:
                Debug.Log("Increase Gold for next fight.");
                break;

            case ItemEffect.StatBoost:
                Debug.Log("Increase Attack for next fight.");
                break;

            case ItemEffect.SpawnBoost:
                Debug.Log("Less enemy will appear for next fight.");
                break;

            default:
                Debug.Log("No effect.");
                break;
        }
    }

    public void CancelSelection() => ResetSelection();

    private void ResetSelection()
    {
        selectedIndex = -1;

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].ItemName.color = Color.black;

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

        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].ItemName.color = Color.black;

        confirmButton.interactable = false;
        cancelButton.interactable = false;

        RandomizeShopItems();
    }
}
