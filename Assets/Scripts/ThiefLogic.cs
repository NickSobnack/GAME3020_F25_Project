using UnityEngine;
using System.Collections;

public class ThiefLogic : EnemyBase
{
    /* 
    Thieves will steal gold from the player.
    Perfect Guard nullifies this.
    Regular Guard = regular amount.
    No block = more stolen.
    
    */

    [Header("Thief Attack")]
    private float swipeTimer;
    private float timer;
    private CircleCollider2D collider;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float distanceToPlayer = 2.5f;
    [SerializeField] private float returnDelay = 0.5f;

    private Vector3 originalPos;
    private Transform player;
    private bool isStunned = false;

    protected override void Awake()
    {
        base.Awake();
        swipeTimer = Random.Range(3f, 6f);
        base.damage = 4f;

        player = GameObject.FindWithTag("Player").transform;
        collider = GetComponent<CircleCollider2D>();
        originalPos = transform.position;
    }

    private void Update()
    {
        if (isStunned) return;

        timer += Time.deltaTime;
        if (timer >= swipeTimer)
        {
            if (!battleLogic.inAction)
                StartCoroutine(ChargeAttack());

            timer = 0f;
            swipeTimer = Random.Range(3f, 6f);
        }
    }

    // Thief dashes toward player, swipes, then returns
    private IEnumerator ChargeAttack()
    {
        inAction = true;
        battleLogic.SetInAction(true);

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 attackPos = player.position - direction * distanceToPlayer;

        yield return MoveToPosition(attackPos);
        yield return new WaitForSeconds(0.3f);

        animator.SetTrigger("Swipe");
        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(returnDelay);
        collider.enabled = false;

        yield return MoveToPosition(originalPos);

        collider.enabled = true;
        inAction = false;
        battleLogic.SetInAction(false);
    }

    // Smooth movement to target position
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
