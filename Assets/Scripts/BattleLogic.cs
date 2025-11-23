using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleLogic : MonoBehaviour
{
    // -------------------- Simple Attack & Block Mechanic -------------------- //
    // Energy system where attacking or holding block consumes NRG.
    // Attacking costs a fixed amount while holding block drains NRG over time.
    // When NRG hits 0, it regens slower, prevents actions, else regens faster.

    private PlayerInput playerInput;
    private InputAction attackAction, blockAction, targetAction;

    [Header("Energy System")] 
    [SerializeField] private float maxEnergy = 10f;
    [SerializeField] private float energyRegen = 1f;
    [SerializeField] private float blockCost = 1f;
    [SerializeField] private float attackCost = 3f;
    [SerializeField] private float currEnergy;

    private float currHoldTimer;
    private bool isAttacking, isBlocking, noEnergy;

    [Header("UI")]
    public Image energyBar; 

    [Header("Animation")]
    public Animator playerAnimator;
    public Transform gameStatusVfxPoint;
    public GameObject perfectVfx;
    public float currBlockTime { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float distanceToEnemy = 1.5f;
    [SerializeField] private float returnDelay = 0.5f;
    private Vector3 originalPos;
    private Vector3 attackPos;

    [Header("Enemy")]
    [SerializeField] private List<EnemyBase> enemies = new(); 
    [SerializeField] private GameObject targetPointerPrefab;
    [SerializeField] public float playerDmg = 9f;
    private GameObject targetPointerInstance;
    private EnemyBase selectedTarget;
    private int selectedIndex = 0;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blockAction = playerInput.actions["Block"];
        attackAction = playerInput.actions["Attack"]; 
        targetAction = playerInput.actions["Target"];
        currEnergy = maxEnergy;
        noEnergy = false;
    }

    private void OnEnable()
    {
        attackAction.started += ctx => Attack();
        attackAction.Enable();

        blockAction.started += ctx => StartBlock();
        blockAction.canceled += ctx => EndBlock(false);
        blockAction.Enable();

        targetAction.started += ctx => SelectTarget();
        targetAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.started -= ctx => Attack();
        attackAction.Disable();

        blockAction.started -= ctx => StartBlock();
        blockAction.canceled -= ctx => EndBlock(false);
        blockAction.Disable();

        targetAction.started -= ctx => SelectTarget();
        targetAction.Disable();
    }

    // Populate enemy list at the start of the scene.
    void Start()
    {
        enemies.AddRange(Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None));
        selectedIndex = 0;
    }

    private void Update()
    {
        if (isBlocking)
        {
            // Drains energy when holding block, if energy hits 0, block ends and locked until energy is fully recharged.
            currHoldTimer += Time.deltaTime;
            currEnergy -= blockCost * Time.deltaTime;
            //
            if (currEnergy < 0) 
                currEnergy = 0;
            if (currEnergy == 0)
            { 
                noEnergy = true; 
                EndBlock(true);
            }
        }
        else if(!isAttacking)
        {
            // If out of energy, it regens at half the normal rate.
            if (noEnergy)
            {
                currEnergy += (energyRegen * 0.5f) * Time.deltaTime;

                if (currEnergy >= maxEnergy)
                {
                    currEnergy = maxEnergy;
                    noEnergy = false;
                }
            }
            // Otherwise, it regens at normal rate when let go early.
            else
            {
                currEnergy += energyRegen * Time.deltaTime;
                currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);
            }
        }
        energyBar.fillAmount = currEnergy / maxEnergy;
    }

    // Attack the selected target if not blocking, has enough energy, and not already attacking.
    // If all enemies are defeated, trigger win message and load next scene.
    private void Attack()
    {
        if (!isBlocking && currEnergy >= attackCost && !isAttacking && selectedTarget != null)
        {
            currEnergy -= attackCost;
            isAttacking = true;
            originalPos = transform.position;
            StartCoroutine(AttackSequence(selectedTarget));
        }
    }

    // Block function so that when block key is held and player has energy, the block animation plays.
    private void StartBlock()
    {
        if (!isBlocking && !noEnergy && currEnergy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
            currBlockTime = Time.time;
            playerAnimator.SetBool("isBlocking", true);
        }
    }

    // End block when block key is released or energy hits 0, and block animation stops.
    private void EndBlock(bool autoRelease)
    {
        if (isBlocking)
        {
            isBlocking = false;
            playerAnimator.SetBool("isBlocking", false);
        }
    }

    // Animation event triggers after attack animation finishes, to prevent attack spam.
    public void AttackFinished()
    {
        isAttacking = false;
    }
    public void RestoreEnergy(float amount)
    {
        currEnergy += amount;
        currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);
        energyBar.fillAmount = currEnergy / maxEnergy;
    }


    // Cycles through available enemies as target when tab is pressed.
    private void SelectTarget()
    {
        if (selectedTarget != null)
            selectedTarget.SetSelected(false);

        enemies.RemoveAll(e => e == null || e.health <= 0);

        if (enemies.Count == 0)
        {
            selectedTarget = null;
            return;
        }

        selectedIndex = (selectedIndex + 1) % enemies.Count;
        selectedTarget = enemies[selectedIndex];

        selectedTarget.SetSelected(true);
    }

    // Coroutine to handle moving to enemy using lerp, play attack animation and deal damage then return to og position.
    private IEnumerator AttackSequence(EnemyBase target)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Vector3 attackPos = target.transform.position - direction * distanceToEnemy;

        yield return MoveToPosition(attackPos);

        playerAnimator.SetTrigger("Attack");
        AudioManager.Instance.PlaySound(SoundName.sword);
        yield return new WaitForSeconds(0.5f);

        target.TakeDamage(playerDmg);
        CheckAllEnemiesDefeated();

        yield return new WaitForSeconds(returnDelay);
        yield return MoveToPosition(originalPos);

        isAttacking = false;
    }

    // Lerp movement to selected target position. Works by gradually updating position until close enough.
    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float progress = 0f;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            progress += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.Clamp01(progress));
            yield return null;
        }

        transform.position = targetPos;
    }

    // Check if all enemies are defeated, then play perfect vfx and load next scene after delay.
    private void CheckAllEnemiesDefeated()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.health > 0)
                return;
        }
        Instantiate(perfectVfx, gameStatusVfxPoint.position, Quaternion.identity);
        AudioManager.Instance.PlayMusic(MusicName.victory, false);
        GameManager.Instance.DelayLoadScene(1, 3f);
    }
}