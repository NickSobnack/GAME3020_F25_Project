using UnityEngine;
using UnityEngine.UI;

public class PlayerLogic : MonoBehaviour
{
    // TODO: Add sfx and visual effects for getting hit. 

    private Animator playerAnimator;

    [Header("Player Properties")]
    public float maxHealth = 10;
    public float damage = 2;
    private float health;
    private bool playerDead = false;

    [Header("UI")]
    public Image healthBar;
    public GameObject fadePanel;

    [Header("VFX Properties")]
    public Transform blockVfxPoint;
    public Transform gameOverVfxPoint;
    public GameObject niceVfx, gameOverVfx;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        health = maxHealth;
    }

    private void Update()
    {
        healthBar.fillAmount = health / maxHealth;

        if (health <= 0 && !playerDead)
        {
            OnPlayerDeath();
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
                Instantiate(niceVfx, blockVfxPoint.position, Quaternion.identity);
            }
            else
            {
                //Instantiate(missedVFX, vfxPoint.position, Quaternion.identity);
                health -= damage;
                health = Mathf.Clamp(health, 0, maxHealth);
            }
            Destroy(collision.gameObject); 
        }
    }

    void OnPlayerDeath()
    {
        playerDead = true;
        fadePanel.SetActive(true);
        Instantiate(gameOverVfx, gameOverVfxPoint.position, Quaternion.identity);
    }
}
