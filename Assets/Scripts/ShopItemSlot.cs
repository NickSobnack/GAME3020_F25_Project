using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Button itemButton;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBorder;

    public Button ItemButton => itemButton;
    public TMP_Text ItemName => itemName;
    public Image ItemIcon => itemIcon;
    public Image ItemBorder => itemBorder;
}
