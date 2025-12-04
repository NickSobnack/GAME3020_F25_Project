using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossLogic : EnemyBase, IDamage
{
    [Header("Boss Stats")]
    public int maxShield = 15;
    public int currentShield;

    public float attackInterval = 3f; 
    private float attackTimer;

    public float moveSpeed = 5f;
    public float attackRange = 2f;
    public float bulletSpeed = 15f;

    private Transform player;
    private Vector3 originalPosition;

    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Slider hpSliderOverride;
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

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        shieldSlider.maxValue = maxShield;
        shieldSlider.value = currentShield;
        
        hpSlider = hpSliderOverride; 
        hpSlider.maxValue = maxHealth;
        hpSlider.value = health;
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

        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(100f);
        }
    }

    private void PerformRandomAttack()
    {
        int attackChoice = Random.Range(0, 2);

        if (attackChoice == 0)
        {
            RangeAttack();
        }
        else
        {
            if (player != null)
                StartCoroutine(PhysicalAttack());
        }
    }

    public void RangeAttack()
    {
        animator.SetTrigger("Slash");
    }

    // Animation event called at the end of slash attack.
    public void SpawnSlash()
    {
        slashRb = Instantiate(slashPrefab, transform.position, transform.rotation);
        slashRb.linearVelocity = slashRb.transform.right * -1 * bulletSpeed;
        slashProjectile = slashRb.gameObject.GetComponent<Projectile>();
        slashProjectile.enemyCollider = bossCollider;
    }

    private IEnumerator PhysicalAttack()
    {
        inAction = true;
        yield return StartCoroutine(MoveToPosition(player.position, attackRange));

        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(MoveToPosition(originalPosition, 0.1f));

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
            Debug.Log("Boss shield absorbed the attack. No HP damage.");
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
            if (currentShield < 0) currentShield = 0;

            shieldSlider.value = currentShield;
            Debug.Log("Boss shield: " + currentShield);

            if (currentShield == 0)
            {
                Debug.Log("Boss shield is broken.");
                shieldSlider.gameObject.SetActive(false);
                hpSlider.gameObject.SetActive(true);
            }
            return;
        }

        base.TakeDamage(damageAmount);
        hpSlider.value = health;
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
