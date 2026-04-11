using UnityEngine;

public class SpellLogic : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    private Vector2 direction;

    [Header("Growth Settings")]
    [SerializeField] private float growthAmount = 0.5f;   
    [SerializeField] private float timeBetweenGrowths = 0.5f;
    [SerializeField] private int totalGrowthStages = 3;

    public float damage = 5f;

    private Rigidbody2D rb;
    private int growthsDone = 0;
    private float growthTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleGrowth();
    }

    private void HandleGrowth()
    {
        if (growthsDone >= totalGrowthStages)
            return;

        growthTimer += Time.deltaTime;

        if (growthTimer >= timeBetweenGrowths)
        {
            growthTimer = 0f;
            growthsDone++;

            // Apply growth
            transform.localScale += new Vector3(growthAmount, growthAmount, 0f);
            AudioManager.Instance.PlaySound(SoundName.spell);
        }
    }

    public void Launch(Vector2 targetPosition)
    {
        direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

}
