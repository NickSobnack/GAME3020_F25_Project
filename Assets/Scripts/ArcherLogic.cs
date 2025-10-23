using UnityEngine;

public class ArcherLogic : EnemyBase
{
    [Header("Archer Attack")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float arrowSpeed = 10f;
    private float shootTimer;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        shootTimer = Random.Range(2f, 4f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootTimer)
        {
            animator.SetTrigger("Shoot");
            timer = 0f;
            shootTimer = Random.Range(2f, 4f);
        }
    }

    public void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * arrowSpeed;
        Destroy(arrow, 3f);
    }

    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hit");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}
