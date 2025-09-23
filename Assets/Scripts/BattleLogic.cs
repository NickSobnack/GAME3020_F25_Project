using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BattleLogic : MonoBehaviour
{
    // -------------------- Simple Attack & Block Mechanic -------------------- //
    // Energy system where attacking or holding block consumes NRG.
    // Attacking costs a fixed amount while holding block drains NRG over time.
    // When NRG hits 0, it regens slower, prevents actions, else regens faster.

    private PlayerInput playerInput;
    private InputAction attackAction, blockAction;

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

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blockAction = playerInput.actions["Block"];
        attackAction = playerInput.actions["Attack"];
        playerAnimator.SetBool("isBlocking", false);
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

    private void Attack()
    {
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

    // Animation event triggers after attack animation finishes, to prevent attack spam.
    public void AttackFinished()
    {
        isAttacking = false;
    }
}