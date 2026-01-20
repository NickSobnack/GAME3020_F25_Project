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
            int roll = Random.Range(0, 3);

            if (roll == 1)
            {
                AudioManager.Instance.PlaySound(SoundName.hurt1);
            }
            else if (roll == 2)
            {
                AudioManager.Instance.PlaySound(SoundName.hurt2);
            }
            else
            {
                AudioManager.Instance.PlaySound(SoundName.hurt3);
            }
        }
    }

    protected abstract void PlayHurtAnimation();
    protected abstract void PlayDeathAnimation();

    public virtual void OnDeath()
    {
        PlayDeathAnimation();

        if (hpSlider != null)
            hpSlider.value = 0;

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

}
