using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleLogic : MonoBehaviour
{
    // ----------------- Simple Press and Hold Block Mechanic ----------------- //
    // Hold the block button to block incoming attacks for a set amount of time.  
    // If held for too long, block will automatically release and go on cooldown. 
    // If let go early, block goes on cooldown immediately.

    private PlayerInput playerInput;
    private InputAction blockAction;
    private bool isBlocking;

    [Header("Player Attribute")]
    [SerializeField] private float currHoldTimer;
    [SerializeField] private float maxHoldTimer = 2f;
    [SerializeField] private float cooldownTimer;   
    [SerializeField] private float blockCooldown = 2f;

    // Energy system where holding block consumes energy over time.
    // Letting go regens energy at a normal rate. 
    // If energy depletes, block is forced to end, cannot be used and energy regens slowly.

    [Header("Energy System")]
    [SerializeField] private float maxEnergy = 10f;
    [SerializeField] private float currEnergy;
    [SerializeField] private float energyRegen = 1f;
    [SerializeField] private float energyDrain = 1f;
    private bool noEnergy;

    [Header("UI")]
    public TextMeshProUGUI cooldownText;  
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        blockAction = playerInput.actions["Block"];
        currEnergy = maxEnergy;
        noEnergy = false;
    }

    private void OnEnable()
    {
        blockAction.started += ctx => StartBlock();
        blockAction.canceled += ctx => EndBlock(false);
        blockAction.Enable();
    }

    private void OnDisable()
    {
        blockAction.started -= ctx => StartBlock();
        blockAction.canceled -= ctx => EndBlock(false);
        blockAction.Disable();
    }

    private void Update()
    {
        // Reduce cooldown timer if active.
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // If block is held, count and check for max hold time.
        // If max hold time is reached, auto-release the block.
        if (isBlocking)
        {
            currHoldTimer += Time.deltaTime;
            if (currHoldTimer >= maxHoldTimer)
            {
                EndBlock(true);
            }
        }
        
        // Simple cooldown update in real time.
        if (cooldownTimer > 0)
        {
            cooldownText.text = $"Cooldown: {cooldownTimer:F1}s";
        }
        else
        {
            cooldownText.text = "Block Ready!";
        }
    }

    private void StartBlock()
    {
        if (!isBlocking && !noEnergy && currEnergy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
            Debug.Log("Blocking.");
        }
        else if (noEnergy || currEnergy == 0)
        {
            Debug.Log("Out of energy.");
        }
    }

    private void EndBlock(bool autoRelease)
    {
        if (isBlocking)
        {
            isBlocking = false;
            //cooldownTimer = blockCooldown;

            if (autoRelease)
                Debug.Log("Block on cd after max hold time.");
            
            else
                Debug.Log($"Block on cd after {currHoldTimer:F2} seconds.");
        }
    }
}