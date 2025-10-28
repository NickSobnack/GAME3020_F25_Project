using UnityEngine;

public class ArcherLogic : EnemyBase
{
    [Header("Archer Attack")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
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
            shootTimer = Random.Range(3f, 6f);
        }
    }

    // Fire an arrow projectile.
    public void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
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
