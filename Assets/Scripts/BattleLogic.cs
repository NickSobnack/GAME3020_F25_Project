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

    // TO DO: Add visual for enemy health and pace timing between retreat animation and win message.

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

    [Header("Enemy")]
    [SerializeField] private List<EnemyBase> enemies = new(); 
    private EnemyBase selectedTarget;
    private int selectedIndex = 0;
    [SerializeField] private GameObject targetPointerPrefab;
    private GameObject targetPointerInstance;
    [SerializeField] public float dmg;

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
            playerAnimator.SetTrigger("Attack");

            selectedTarget.TakeDamage(dmg);

            CheckAllEnemiesDefeated(); 
        }
    }

    // Block function so that when block key is held and player has energy, the block animation plays.
    private void StartBlock()
    {
        if (!isBlocking && !noEnergy && currEnergy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
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


    // Check if all enemies are defeated, then play perfect vfx and load next scene after delay.
    private void CheckAllEnemiesDefeated()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.health > 0)
                return;
        }
        Instantiate(perfectVfx, gameStatusVfxPoint.position, Quaternion.identity);
        GameManager.Instance.DelayLoadScene(1, 3f);
    }
}