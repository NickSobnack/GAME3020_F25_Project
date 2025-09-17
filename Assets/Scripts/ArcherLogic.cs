using UnityEngine;

public class ArcherLogic : MonoBehaviour
{
    [Header("Animation")]
    Animator animator;

    [Header("Arrow Properties")]
    private float timer = 0f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint, player;
    [SerializeField] private float shootTimer = 2f;
    [SerializeField] private float arrowSpeed = 10f; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Trigger an arrow shot at player every shootTimer seconds.
        timer += Time.deltaTime;
        if (timer >= shootTimer)
        {
            animator.SetTrigger("Shoot");
            timer = 0f;
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
}
