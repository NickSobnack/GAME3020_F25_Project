using UnityEngine;

public enum ItemRarity { Common, Uncommon, Rare } // Spawn chance in shop: Common 70%, Uncommon 25%, Rare 5%.
public enum ItemEffect { None, AttackBoost, DefenceBoost, GoldBoost, SpawnBoost }
// Stat boosts are flat increases to the player's stats.
// i.e Attack adds a flat amt to normal damage and defence boost reduces damage taken.
// Gold boosts adds an extra amount of gold when picking up gold bags.
// Spawn boosts - not implemented.

[CreateAssetMenu(fileName = "ShopItem", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject 
{
    public string id;
    public string itemName;
    public string description;
    public int cost;
    public float effectValue;
    public int duration; 
    public bool isOneTimePurchase;
    public Sprite icon;
    public Color rarityColor;
    public ItemEffect effectType;
    public ItemRarity rarityType;
}
