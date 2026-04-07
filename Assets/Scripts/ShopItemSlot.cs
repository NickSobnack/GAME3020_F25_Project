using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Button itemButton;
    [SerializeField] private Image itemBorder;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image itemIcon;

    public Button ItemButton => itemButton;
    public Image ItemBorder => itemBorder;
    public TMP_Text ItemName => itemName;
    public Image ItemIcon => itemIcon;
}
