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
    [SerializeField] private float energyDrain = 1f;
    private float currEnergy, currHoldTimer;
    private bool isBlocking, noEnergy;

    [Header("UI")]
    public Image energyBar; 

    [Header("Animation")]
    public Animator playerAnimator; 

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blockAction = playerInput.actions["Block"];
        attackAction = playerInput.actions["Attack"];
        currEnergy = maxEnergy;
        noEnergy = false;
        playerAnimator.SetBool("isBlocking", false); 
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
            currHoldTimer += Time.deltaTime;
            // Energy drains when holding block.
            currEnergy -= energyDrain * Time.deltaTime;
            // If energy reaches 0, block ends and locked until energy is fully recharged.
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
    }

    private void Attack()
    {
        // Attack animation plays when attack key is tapped and while not blocking.
        if (!isBlocking) playerAnimator.SetTrigger("Attack"); 
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
        else if (noEnergy || currEnergy == 0) Debug.Log("Out of energy.");
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
}