using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    // Enemy base class for shared functionality among enemies like health, damage handling, and death.
    [Header("Enemy Stats")]
    public float maxHealth = 9f;
    public float health;
    public float damage = 3f;
    public GameObject targetPointer;
   
    [HideInInspector] 
    public string enemyName;
    protected Animator animator;
    protected BattleLogic battleLogic;

    public bool inAction { get; protected set; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        battleLogic = FindObjectOfType<BattleLogic>();
        if (battleLogic != null)
            battleLogic.RegisterEnemy(this);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            OnDeath();
        else
            PlayHurtAnimation();
    }

    protected abstract void PlayHurtAnimation();
    protected abstract void PlayDeathAnimation();

    public virtual void OnDeath()
    {
        PlayDeathAnimation();
        Destroy(gameObject, 1f);
    }

    public void SetSelected(bool isSelected)
    {
        if (targetPointer != null)
            targetPointer.SetActive(isSelected);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log("Hit by projectile:" + other.gameObject);
        }
    }

}
