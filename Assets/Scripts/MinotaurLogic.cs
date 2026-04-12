using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MinotaurLogic : EnemyBase, IDamage
{
    [Header("Minotaur Stats")]
    public float moveSpeed = 6f;
    public float attackRange = 1.5f;
    public float chargeSpeed = 12f;

    [Header("Attack Settings")]
    public float attackInterval = 4f;
    private float attackTimer;

    [Header("Charge Settings")]
    public float chargeWindupDuration = 1.2f;
    public float chargeDuration = 0.6f;

    [Header("UI")]
    [SerializeField] private Slider hpSliderOverride;

    [Header("FX")]
    [SerializeField] private ParticleSystem chargeUpParticle;

    private Transform player;
    private Vector3 originalPosition;

    protected override void Awake()
    {
        base.Awake();
        base.damage = 8f;

        originalPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        hpSlider = hpSliderOverride;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = health;

        if (chargeUpParticle != null)
            chargeUpParticle.Stop();
    }

    private void Start()
    {
        attackTimer = attackInterval;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            StartCoroutine(ChargeAttack());
            attackTimer = attackInterval;
        }
    }

    private IEnumerator ChargeAttack()
    {
        inAction = true;

        animator.SetTrigger("Smash");

        if (chargeUpParticle != null)
            chargeUpParticle.Play();

        yield return new WaitForSeconds(chargeWindupDuration);

        if (chargeUpParticle != null)
            chargeUpParticle.Stop();

        if (player != null)
        {
            Vector3 chargeTarget = player.position;
            float elapsed = 0f;

            while (elapsed < chargeDuration &&
                   Vector3.Distance(transform.position, chargeTarget) > attackRange)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, chargeTarget, chargeSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.4f);
        yield return MoveToPosition(originalPosition, 0.1f);

        inAction = false;
    }

    private IEnumerator MoveToPosition(Vector3 targetPos, float stopDistance)
    {
        while (Vector3.Distance(transform.position, targetPos) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        hpSlider.value = health;
    }

    public void Damage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
        hpSlider.value = health;
    }

    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}