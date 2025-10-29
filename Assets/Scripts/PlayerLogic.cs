using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class PlayerLogic : MonoBehaviour
{
    // TODO: Add sfx and visual effects for getting hit. 
    // TODO: Attack animations.

    private Animator playerAnimator;

    [Header("Player Properties")]
    public float maxHealth = 10;
    public float damage = 2;
    private float health;
    private bool playerDead = false;
    public float perfectBlockWindow = 0.2f;
    private BattleLogic battleLogic;

    [Header("UI")]
    public Image healthBar;
    public GameObject fadePanel;

    [Header("VFX Properties")]
    public Transform blockVfxPoint;
    public Transform gameOverVfxPoint;
    public GameObject goodVfx, niceVfx, gameOverVfx;

    // Set initial values for player hp.
    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        battleLogic = GetComponent<BattleLogic>();
        health = maxHealth;
    }

    // Update keeps track of player health and updates health bar UI.
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (playerAnimator.GetBool("isBlocking"))
            {
                float timeSinceBlock = Time.time - battleLogic.currBlockTime;
                if (timeSinceBlock <= perfectBlockWindow)
                {
                    Instantiate(niceVfx, blockVfxPoint.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(goodVfx, blockVfxPoint.position, Quaternion.identity);
                }
            }
            else
            {
                health -= damage;
                health = Mathf.Clamp(health, 0, maxHealth);
            }
            Destroy(other.gameObject);
        }
    }


    // Function that gets called when player hp reaches 0 and triggers game over sequence.
    void OnPlayerDeath()
    {
        playerDead = true;
        fadePanel.SetActive(true);
        Instantiate(gameOverVfx, gameOverVfxPoint.position, Quaternion.identity);
    }
}
