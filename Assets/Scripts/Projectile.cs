using UnityEngine;

public class Projectile : MonoBehaviour, IDeflect
{
    [SerializeField] public float damageAmount = 3f;
    private IDamage damageable;
    private Collider2D projectileCollider;
    private Rigidbody2D projectileRb;
    private SpriteRenderer projectileSprite;
    private bool isDeflected = false;

    public Collider2D enemyCollider { get; set; }

    [field : SerializeField] public float ReturnSpeed { get; set; } = 10f;

    private void Start()
    {
        projectileCollider = GetComponent<Collider2D>();
        projectileRb = GetComponent<Rigidbody2D>();
        projectileSprite = GetComponent<SpriteRenderer>();
        IgnoreCollisionWithEnemy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamage damageable = collision.GetComponent<IDamage>();
        if (damageable != null)
        {
            // If deflected, damage enemies
            if (isDeflected && collision.CompareTag("Enemy"))
            {
                damageable.Damage(damageAmount);
                Destroy(gameObject);
            }
        }
    }

    // Toggles collision ignoring between the projectile and the enemy collider.
    private void IgnoreCollisionWithEnemy()
    {
        if (!Physics2D.GetIgnoreCollision(projectileCollider, enemyCollider))   
            Physics2D.IgnoreCollision(projectileCollider, enemyCollider, true);
        else
            Physics2D.IgnoreCollision(projectileCollider, enemyCollider, false);
    }

    public void Deflect(Vector2 direction)
    {
        isDeflected = true;
        IgnoreCollisionWithEnemy(); 
        Collider2D playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(projectileCollider, playerCollider, true);
        projectileRb.linearVelocity = direction * ReturnSpeed;
        projectileSprite.flipX = direction.x < 0;
    }
}
