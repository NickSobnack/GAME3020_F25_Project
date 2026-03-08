using UnityEngine;

// Stores the gold value when dropped and adds it to player's gold count.
public class MoneyBag : MonoBehaviour
{
    public int goldValue;

    private void OnMouseDown()
    {
        GameManager.Instance.AddGold(goldValue);
        Destroy(gameObject);
    }
}