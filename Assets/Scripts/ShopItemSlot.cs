using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Updated to pass item data to tooltip and display to user.
public class ShopItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button itemButton;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBorder; 

    private ItemData itemData;

    public Button ItemButton => itemButton;

    public TMP_Text ItemName => itemName;
    public Image ItemIcon => itemIcon;
    public Image ItemBorder => itemBorder;
    public ItemData ItemData => itemData;

    public void SetItem(ItemData data)
    {
        itemData = data;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TooltipManager.Instance.tooltip.SetTooltip(itemData.cost.ToString() + "g", itemData.description);
        TooltipManager.Show();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TooltipManager.Hide();
    }

}
