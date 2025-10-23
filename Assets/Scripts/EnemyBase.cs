using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 10f;
    public float health;

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
        else
        {
            PlayHurtAnimation();
        }
    }

    protected abstract void PlayHurtAnimation();
    protected abstract void PlayDeathAnimation();

    public virtual void OnDeath()
    {
        PlayDeathAnimation();
        Destroy(gameObject, 1f);
    }
}
