using UnityEngine;

public class ArrowLogic : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float arcHeight = 0.5f;    // Adjusts the height of the arc.
    public float damage = 2f;
    private Rigidbody2D rb;

    private void Awake()
    {   // Enable gravity so the arrow arcs downward.
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
    }

    private void Update()
    {
        if (rb != null && rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Launch(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        direction.y += arcHeight;
        direction = direction.normalized;
        rb.linearVelocity = direction * speed;
    }
}
