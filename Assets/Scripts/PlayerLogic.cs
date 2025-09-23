using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private Animator playerAnimator;

    [Header("VFX Properties")]
    public Transform vfxPoint;
    public GameObject niceVFX, missedVFX;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
    }

    // Detect collisions with other game objects and reacts accordingly.
    // If colliding with an enemy tag, check if blocking animation is on/off and displays vfx.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (playerAnimator.GetBool("isBlocking"))
            {
                Instantiate(niceVFX, vfxPoint.position, Quaternion.identity);
            }
            else
            {
                Instantiate(missedVFX, vfxPoint.position, Quaternion.identity);
            }
            Destroy(collision.gameObject); 
        }
    }
}
