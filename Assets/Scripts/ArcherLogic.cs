using UnityEngine;
using System.Collections;

public class ArcherLogic : EnemyBase
{
    [Header("Archer Attack")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    private float shootTimer;
    private float timer;
    private bool isStunned = false;

    protected override void Awake()
    {
        base.Awake();
        shootTimer = Random.Range(2f, 4f);
    }

    private void Update()
    {
        if (isStunned) return;

        timer += Time.deltaTime;
        if (timer >= shootTimer)
        {
            if (!battleLogic.inAction && !inAction)
                StartCoroutine(ShootRoutine());
            
            timer = 0f;
            shootTimer = Random.Range(3f, 6f);
        }
    }

    // Animation event that gets called at end of Shoot animation to fire an arrow projectile.
    public void FireArrow()
    {
        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        ArrowLogic arrow = arrowObj.GetComponent<ArrowLogic>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2 playerPos = player.transform.position;

        arrow.Launch(playerPos);

        AudioManager.Instance.PlaySound(SoundName.bow);
        Destroy(arrowObj, 3f);
    }

    private IEnumerator ShootRoutine()
    {
        inAction = true;
        battleLogic.SetInAction(true);

        animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.3f);

        inAction = false;
        battleLogic.SetInAction(false);
    }

    protected override void PlayHurtAnimation()
    {
        isStunned = true;
        animator.SetTrigger("Hit");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }

    public void EndStun()
    {
        isStunned = false;
    }
}
