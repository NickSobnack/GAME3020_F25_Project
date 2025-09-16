using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleLogic : MonoBehaviour
{
    // ----------------- Simple Press and Hold Block Mechanic ----------------- //
    // Energy system where holding block consumes energy over time.
    // Letting go of block regens energy at a normal rate. 
    // When energy hits 0, block auto ends & goes on cd, energy regens slower.

    private PlayerInput playerInput;
    private InputAction attackAction, blockAction;

    [Header("Energy System")] 
    [SerializeField] private float maxEnergy = 10f;
    [SerializeField] private float energyRegen = 1f;
    [SerializeField] private float blockCost = 1f;
    [SerializeField] private float currEnergy;
    private float currHoldTimer;
    private bool isAttacking, isBlocking, noEnergy;
    public float attackCostModifier;

    [Header("UI")]
    public Image energyBar; 

    [Header("Animation")]
    public Animator playerAnimator; 

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blockAction = playerInput.actions["Block"];
        attackAction = playerInput.actions["Attack"];
        playerAnimator.SetBool("isBlocking", false);
        currEnergy = maxEnergy;
        noEnergy = false;
        attackCostModifier = 3f;
    }

    private void OnEnable()
    {
        attackAction.started += ctx => Attack();
        attackAction.Enable();

        blockAction.started += ctx => StartBlock();
        blockAction.canceled += ctx => EndBlock(false);
        blockAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.started -= ctx => Attack();
        attackAction.Disable();

        blockAction.started -= ctx => StartBlock();
        blockAction.canceled -= ctx => EndBlock(false);
        blockAction.Disable();
    }

    private void Update()
    {
        if (isBlocking)
        {
            // Drains energy when holding block, if energy hits 0, block ends and locked until energy is fully recharged.
            currHoldTimer += Time.deltaTime;
            currEnergy -= blockCost * Time.deltaTime;
            //
            if (currEnergy < 0) currEnergy = 0;
            if (currEnergy == 0) noEnergy = true;
        }
        else
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

        // ----------------- CHEATS ----------------- //
        if (Input.GetKeyDown(KeyCode.U))
        {
            Upgrade();
        }
        // ----------------- CHEATS ----------------- //
    }

    private void Attack()
    {
        float attackCost = maxEnergy / attackCostModifier;

        // Can attack only if not blocking or attacking and has enough energy.
        if (!isBlocking && currEnergy >= attackCost && !isAttacking)
        {
            currEnergy -= attackCost;
            isAttacking = true;
            playerAnimator.SetTrigger("Attack");
        }
    }

    private void StartBlock()
    {
        // Start blocking when block key is held and has energy, and block animation plays.
        if (!isBlocking && !noEnergy && currEnergy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
            playerAnimator.SetBool("isBlocking", true);
        }
    }

    private void EndBlock(bool autoRelease)
    {
        // End block when block key is released or energy hits 0, and block animation stops.
        if (isBlocking)
        {
            isBlocking = false;
            playerAnimator.SetBool("isBlocking", false);
        }
    }

    public void Upgrade()
    {
        attackCostModifier = 6f;
    }

    // Animation event triggers after attack animation finishes, to prevent attack spam.
    public void AttackFinished()
    {
        isAttacking = false;
    }
}