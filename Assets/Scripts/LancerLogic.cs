using UnityEngine;

public class LancerLogic : EnemyBase
{
    [Header("Lancer Attack")]
    private float thrustTimer;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        thrustTimer = Random.Range(3f, 6f);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= thrustTimer)
        {
            animator.SetTrigger("Thrust");
            timer = 0f;
            thrustTimer = Random.Range(2f, 4f);
        }
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
