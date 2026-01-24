using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossLogic : EnemyBase, IDamage
{
    [Header("Boss Stats")]
    public int maxShield = 15;
    public int currentShield;

    private int meleeAttackStreak = 0;
    private bool lastAttackWasRanged = false;

    [Header("Attack Settings")]
    public float attackInterval = 3f;
    private float attackTimer;

    public float moveSpeed = 5f;
    public float attackRange = 2f;
    public float bulletSpeed = 15f;

    private Transform player;
    private Vector3 originalPosition;

    [Header("UI")]
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Slider hpSliderOverride;

    [Header("Prefabs")]
    [SerializeField] private Rigidbody2D slashPrefab;

    private Rigidbody2D slashRb;
    private Projectile slashProjectile;
    private Collider2D bossCollider;

    public bool HasShield => currentShield > 0;

    protected override void Awake()
    {
        base.Awake();
        base.damage = 5f;

        currentShield = maxShield;
        originalPosition = transform.position;
        bossCollider = GetComponent<Collider2D>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        shieldSlider.maxValue = maxShield;
        shieldSlider.value = currentShield;

        hpSlider = hpSliderOverride;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = health;
    }

    private void Start()
    {
        attackTimer = attackInterval;
        Debug.Log($"Boss spawned with HP: {health} and Shield: {currentShield}");
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            PerformAttackPattern();
            attackTimer = attackInterval;
        }
    }

    private void PerformAttackPattern()
    {
        if (meleeAttackStreak >= 3)
        {
            UseRangeSkill();
            return;
        }

        int roll = Random.Range(0, 2);

        if (lastAttackWasRanged)
        {
            UseMeleeSkill();
            return;
        }

        if (roll == 0)
            UseMeleeSkill();
        else
            UseRangeSkill();
    }

    private void UseMeleeSkill()
    {
        if (player != null)
            StartCoroutine(MeleeAttack());

        meleeAttackStreak++;
        lastAttackWasRanged = false;
    }

    private void UseRangeSkill()
    {
        RangeAttack();
        meleeAttackStreak = 0;
        lastAttackWasRanged = true;
    }


    public void RangeAttack()
    {
        animator.SetTrigger("Slash");
    }

    public void SpawnSlash() // Animation Event
    {
        slashRb = Instantiate(slashPrefab, transform.position, transform.rotation);
        slashRb.linearVelocity = slashRb.transform.right * -1 * bulletSpeed;

        slashProjectile = slashRb.GetComponent<Projectile>();
        slashProjectile.enemyCollider = bossCollider;
    }

    private IEnumerator MeleeAttack()
    {
        inAction = true;

        yield return MoveToPosition(player.position, attackRange);

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);

        yield return MoveToPosition(originalPosition, 0.1f);

        inAction = false;
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
        if (HasShield)
        {
            Debug.Log("Boss shield absorbed the attack.");
            AudioManager.Instance.PlaySound(SoundName.impact);
            return;
        }

        base.TakeDamage(damage);
        hpSlider.value = health;
    }

    public void Damage(float damageAmount)
    {
        if (currentShield > 0)
        {
            currentShield -= (int)damageAmount;
            currentShield = Mathf.Max(0, currentShield);

            shieldSlider.value = currentShield;
            Debug.Log($"Boss shield: {currentShield}");

            if (currentShield == 0)
            {
                Debug.Log("Boss shield broken.");
                shieldSlider.gameObject.SetActive(false);
                hpSlider.gameObject.SetActive(true);
            }

            return;
        }

        base.TakeDamage(damageAmount);
        hpSlider.value = health;
    }

    protected override void PlayHurtAnimation() 
    { }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Run");
    }
}
