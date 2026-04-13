using UnityEngine;

public enum ItemRarity { Common, Uncommon, Rare } // Spawn chance in shop: Common 70%, Uncommon 25%, Rare 5%.
public enum ItemEffect { None, StatBoost, GoldBoost, SpawnBoost }

[CreateAssetMenu(fileName = "ShopItem", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject 
{
    public string id;
    public string itemName;
    public string description;
    public int cost;
    public float effectValue;
    public bool isOneTimePurchase;
    public Sprite icon;
    public Color rarityColor;
    public ItemEffect effectType;
    public ItemRarity rarityType;
}
