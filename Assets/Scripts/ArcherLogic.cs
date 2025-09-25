using UnityEngine;

public class ArcherLogic : MonoBehaviour
{
    // TO DO: Add visual for enemy taking damage.

    [Header("Animation")]
    Animator animator;

    [Header("Archer Properties")]
    private float timer = 0f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint, player;
    [SerializeField] private float shootTimer;
    [SerializeField] private float arrowSpeed = 10f; 
    [SerializeField] private float maxHealth = 9f; 
    [SerializeField] public float health; 

    // Set initial values for shoot timer in between shots and hp.
    private void Awake()
    {
        animator = GetComponent<Animator>();
        shootTimer = Random.Range(2f, 4f);
        health = maxHealth;
    }

    void Update()
    {
        // Trigger an arrow shot at player at a random time interval between 2-4.
        timer += Time.deltaTime;
        if (timer >= shootTimer)
        {
            animator.SetTrigger("Shoot");
            timer = 0f;
            shootTimer = Random.Range(2f, 4f);
            Debug.Log(shootTimer);
        }
    }

    // Animation event that gets called to fire the arrow towards the player.
    public void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * arrowSpeed;
        Destroy(arrow, 3f); 
    }

    // Function that gets called when archer takes damage from player attack,
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    // Function that gets called when archer hp reaches 0, plays run animation and destroys archer object.
    public void OnDeath()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }
}
