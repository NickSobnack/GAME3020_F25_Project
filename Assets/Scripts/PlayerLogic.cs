using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour
{
    // TODO: Add sfx and visual effects for getting hit. 

    private Animator playerAnimator;

    [Header("Player Properties")]
    public float maxHealth = 10;
    private float health;

    [Header("UI")]
    public Image healthBar;

    [Header("VFX Properties")]
    public Transform vfxPoint;
    public GameObject niceVFX;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        health = maxHealth;
    }

    private void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        healthBar.fillAmount = health / maxHealth;

        if (health <= 0)
        {
            OnPlayerDeath();
            Time.timeScale = 0f; 
        }
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
                //Instantiate(missedVFX, vfxPoint.position, Quaternion.identity);
                health -= 2;
            }
            Destroy(collision.gameObject); 
        }
    }

    void OnPlayerDeath()
    {
        Debug.Log("GAME OVER");
    }
}
