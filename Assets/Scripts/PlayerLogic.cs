using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class PlayerLogic : MonoBehaviour
{
    // TODO: Add sfx and visual effects for getting hit. 
    // TODO: Attack animations.

    private Animator playerAnimator;

    [Header("Player Properties")]
    public float maxHealth, health;
    private bool playerDead = false;
    public float perfectBlockWindow = 0.2f;
    public float energyRestore = 2f;
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
        health = GameManager.Instance.playerHealth;
    }

    // Update keeps track of player health and updates health bar UI.
    private void Update()
    {
        healthBar.fillAmount = health / maxHealth;

        GameManager.Instance.playerHealth = health;

        if (health <= 0 && !playerDead)
            OnPlayerDeath();
    }

    // Detect collisions with other game objects and reacts accordingly.
    // If colliding with a bullet (arrow or lance) tag, check if blocking animation is on/off and displays vfx.
    // If not blocking, take appropriate damage from the arrow or lance.
    // If blocking, check if within perfect block window, restore energy and display appropriate vfx.

    private void OnTriggerEnter2D(Collider2D other)
    {
        float damageAmount = 0f;

        if (other.CompareTag("Bullet"))
        {
            ArrowLogic arrow = other.GetComponent<ArrowLogic>();
            if (arrow != null) damageAmount = arrow.damage;

            //LanceLogic lance = other.GetComponent<LanceLogic>();
            //if (lance != null) damageAmount = lance.damage;

            // Destroy projectile after hit
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null) damageAmount = enemy.damage;
        }

        if (damageAmount > 0f)
        {
            if (playerAnimator.GetBool("isBlocking"))
            {
                float timeSinceBlock = Time.time - battleLogic.currBlockTime;
                if (timeSinceBlock <= perfectBlockWindow)
                {
                    Instantiate(niceVfx, blockVfxPoint.position, Quaternion.identity);
                    battleLogic.RestoreEnergy(energyRestore);
                }
                else
                {
                    Instantiate(goodVfx, blockVfxPoint.position, Quaternion.identity);
                }

                AudioManager.Instance.PlaySound(SoundName.bash);
            }
            else
            {
                TakeDamage(damageAmount);
            }
        }
    }

public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    // Function that gets called when player hp reaches 0 and triggers game over sequence.
    void OnPlayerDeath()
    {
        playerDead = true;
        fadePanel.SetActive(true);
        Instantiate(gameOverVfx, gameOverVfxPoint.position, Quaternion.identity);
    }
}
