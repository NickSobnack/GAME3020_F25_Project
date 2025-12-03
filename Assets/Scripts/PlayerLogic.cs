using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerLogic : MonoBehaviour, IDamage
{
    // TODO: Add sfx and visual effects for getting hit. 
    // TODO: Attack animations.

    private Animator playerAnimator;

    [Header("Player Properties")]
    public float maxHealth = 20f;
    public float maxEnergy = 10f;
    public float health, energy;
    public float perfectBlockWindow = 0.2f;
    public float energyRestore = 2f;
    private BattleLogic battleLogic;
    private const string hurtAnim = "Hurt";
    private bool playerDead = false;

    public float MaxHealth => maxHealth;
    public float CurrHealth => health;
    public float MaxEnergy => maxEnergy;
    public float CurrEnergy => energy;


    [Header("UI")]
    public Slider healthSlider;
    public Slider energySlider;
    public float barUpdateSpeed = 5f;
    public GameObject fadePanel;
    private float displayedHealth;
    private float displayedEnergy;

    [Header("VFX Properties")]
    public Transform blockVfxPoint, gameOverVfxPoint;
    public GameObject goodVfx, niceVfx, gameOverVfx;

    // Set initial values for player hp.
    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        battleLogic = GetComponent<BattleLogic>();
        health = GameManager.Instance.playerHealth;
        energy = maxEnergy;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        displayedHealth = health;

        energySlider.maxValue = maxEnergy;
        energySlider.value = energy;
        displayedEnergy = energy;
    }

    // Update keeps track of player health and updates health bar UI.
    private void Update()
    {
        displayedHealth = Mathf.Lerp(displayedHealth, health, Time.deltaTime * barUpdateSpeed);
        healthSlider.value = displayedHealth;

        displayedEnergy = Mathf.Lerp(displayedEnergy, energy, Time.deltaTime * barUpdateSpeed);
        energySlider.value = displayedEnergy;

        GameManager.Instance.playerHealth = health;

        if (health <= 0 && !playerDead)
        {
            OnPlayerDeath();
        }
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

            Destroy(other.gameObject);
        }

        if (other.CompareTag("Projectile"))
        {
            Projectile proj = other.GetComponent<Projectile>();
            if (proj != null)
            {
                if (battleLogic.isDeflecting)
                {
                    return;
                }
                else
                {
                    damageAmount = proj.damageAmount;
                    Destroy(other.gameObject);
                }
            }
        }

        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Collided with Enemy" + other.gameObject);
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && enemy.inAction)  
            {
                damageAmount = enemy.damage;
            }
        }

        if (damageAmount > 0f)
        {
            if (playerAnimator.GetBool("isBlocking"))
            {
                float timeSinceBlock = Time.time - battleLogic.currBlockTime;
                if (timeSinceBlock <= perfectBlockWindow)
                {
                    Instantiate(niceVfx, blockVfxPoint.position, Quaternion.identity);
                    RestoreEnergy(energyRestore);
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
    public void Damage(float damageAmount)
    {
        TakeDamage(damageAmount);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        playerAnimator.SetTrigger(hurtAnim);
    }

    public void HealHealth(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void RestoreEnergy(float amount)
    {
        energy += amount;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    // Function that gets called when player hp reaches 0 and triggers game over sequence.
    void OnPlayerDeath()
    {
        playerDead = true;
        fadePanel.SetActive(true);
        Instantiate(gameOverVfx, gameOverVfxPoint.position, Quaternion.identity);
    }
}
