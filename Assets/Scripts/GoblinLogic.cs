using UnityEngine;
using System.Collections;

public class GoblinLogic : EnemyBase
{
    private float pierceTimer, timer;
    private new CapsuleCollider2D collider;
    private Vector3 originalPos;
    private Transform player;
    private bool isStunned = false;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float distanceToPlayer = 2.5f;
    [SerializeField] private float returnDelay = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        pierceTimer = Random.Range(3f, 6f);
        base.damage = 4f;
        player = GameObject.FindWithTag("Player").transform;
        collider = GetComponent<CapsuleCollider2D>();
        originalPos = transform.position;
    }

    private void Update()
    {
        if (isStunned) return;

        timer += Time.deltaTime;
        if (timer >= pierceTimer)
        {
            if (!battleLogic.inAction)
                StartCoroutine(PierceAttack());

            timer = 0f;
            pierceTimer = Random.Range(3f, 6f);
        }
    }

    private IEnumerator PierceAttack()
    {
        inAction = true;
        battleLogic.SetInAction(true);

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 attackPos = player.position - direction * distanceToPlayer;

        yield return MoveToPosition(attackPos);
        yield return new WaitForSeconds(0.3f);
        animator.SetTrigger("Pierce");
        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(returnDelay);
        collider.enabled = false;
        yield return MoveToPosition(originalPos);

        collider.enabled = true;
        inAction = false;
        battleLogic.SetInAction(false);
    }

    // Lerp movement to selected target position.
    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float progress = 0f;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            progress += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        transform.position = targetPos;
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
