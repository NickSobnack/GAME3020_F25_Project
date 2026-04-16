using UnityEngine;

// Stores the gold value when dropped and adds it to player's gold count.
// Prevents multiple collection.
public class MoneyBag : MonoBehaviour
{
    public int goldValue;
    private bool collected = false;

    private void OnMouseDown()
    {
        if (collected) return;
        collected = true;

        AudioManager.Instance.PlaySound(SoundName.coin);

        int bonus = (int)GameManager.Instance.GetBuffValue(ItemEffect.GoldBoost);
        GameManager.Instance.AddGold(goldValue + bonus);
        Debug.Log($"Collected {goldValue} gold with a bonus of {bonus} gold!");

        Destroy(gameObject, .5f);
    }
}