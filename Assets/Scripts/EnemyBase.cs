using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBase : MonoBehaviour
{
    // Enemy base class for shared functionality among enemies like health, damage handling, and death.
    [Header("Enemy Stats")]
    public float maxHealth = 9f;
    public float health;
    public float damage = 3f;
    public GameObject targetPointer;

    [Header("Loot Drop")]
    public GameObject moneyBagPrefab;
    [Min(0)] public int minGold = 5;
    [Min(0)] public int maxGold = 15;

    [HideInInspector] 
    public string enemyName;
    protected Animator animator;
    protected BattleLogic battleLogic;
    protected Slider hpSlider;

    public bool inAction { get; protected set; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        battleLogic = FindObjectOfType<BattleLogic>();
        if (battleLogic != null)
            battleLogic.RegisterEnemy(this);

        hpSlider = GetComponentInChildren<Slider>(true);
        if (hpSlider == null)
        {
            Transform sliderTransform = transform.Find("HPSlider");
            if (sliderTransform != null)
                hpSlider = sliderTransform.GetComponent<Slider>();
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = health;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (hpSlider != null)
            hpSlider.value = health;

        if (health <= 0)
            OnDeath();
        else
        {
            PlayHurtAnimation();           
        }
    }

    protected abstract void PlayHurtAnimation();
    protected abstract void PlayDeathAnimation();

    public virtual void OnDeath()
    {
        PlayDeathAnimation();

        if (hpSlider != null)
            hpSlider.value = 0;
        DropLoot();
        Destroy(gameObject, 1f);
    }

    public void SetSelected(bool isSelected)
    {
        if (targetPointer != null)
            targetPointer.SetActive(isSelected);
    }

    private void OnMouseDown()
    {
        if (battleLogic != null)
        {
            battleLogic.SelectEnemy(this);
        }
    }

    // Drops a money bag with a random amt of gold which is added to player's when collected.
    protected virtual void DropLoot()
    {
        if (moneyBagPrefab == null) return;

        int goldAmount = Random.Range(minGold, maxGold + 1);

        Vector3 spawnPos = transform.position;
        GameObject bag = Instantiate(moneyBagPrefab, spawnPos, Quaternion.identity);

        MoneyBag moneyBag = bag.GetComponent<MoneyBag>();
        if (moneyBag != null) moneyBag.goldValue = goldAmount;
    }
}
