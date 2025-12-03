using UnityEngine;
using System.Collections;

public class BossLogic : EnemyBase
{
    [Header("Boss Stats")]
    public int maxShield = 50;
    public int currentShield;

    public float attackInterval = 3f; 
    private float attackTimer;

    public float moveSpeed = 5f;
    public float attackRange = 2f;

    private Transform player;
    private Vector3 originalPosition;

    protected override void Awake()
    {
        base.Awake(); 
        currentShield = maxShield;
        originalPosition = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Start()
    {
        attackTimer = attackInterval;
        Debug.Log("Boss spawned with HP: " + health + " and Shield: " + currentShield);
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            PerformRandomAttack();
            attackTimer = attackInterval;
        }
    }

    private void PerformRandomAttack()
    {
        int attackChoice = Random.Range(0, 2);
        if (attackChoice == 0)
        {
            SlashAttack();
        }
        else
        {
            if (player != null)
                StartCoroutine(HeavyAttackRoutine());
        }
    }

    public void SlashAttack()
    {
        animator.SetTrigger("Slash");
    }

    private IEnumerator HeavyAttackRoutine()
    {
        yield return StartCoroutine(MoveToPosition(player.position, attackRange));

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(MoveToPosition(originalPosition, 0.1f));
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, float stopDistance)
    {
        while (Vector3.Distance(transform.position, targetPos) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null; 
        }
    }

    public override void TakeDamage(float damage)
    {
        if (currentShield > 0)
        {
            currentShield -= (int)damage;
            Debug.Log("Boss shield: " + currentShield);

            if (currentShield <= 0)
            {
                Debug.Log("Boss shield is broken.");
            }
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    protected override void PlayHurtAnimation()
    {
        //animator.SetTrigger("Hurt");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Run");
    }
}
