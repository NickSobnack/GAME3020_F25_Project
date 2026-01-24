using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float MaxHealth = 10f;
    public float CurrentHealth { get; set; }
    public float BaseDamage = 3f;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
}
