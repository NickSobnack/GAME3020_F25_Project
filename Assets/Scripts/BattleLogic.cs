using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleLogic : MonoBehaviour
{
    // ----------------- Simple Press and Hold Block Mechanic ----------------- //
    // Energy system where holding block consumes energy over time.
    // Letting go of block regens energy at a normal rate. 
    // When energy hits 0, block auto ends & goes on cd, energy regens slower.

    private PlayerInput playerInput;
    private InputAction blockAction;

    [Header("Energy System")] 
    [SerializeField] private float maxEnergy = 10f;
    [SerializeField] private float energyRegen = 1f;
    [SerializeField] private float energyDrain = 1f;
    private float currEnergy, currHoldTimer;
    private bool isBlocking, noEnergy;

    [Header("UI")]
    public TextMeshProUGUI statusText;  
    
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
                    Debug.Log("Energy fully recharged.");
                }
            }
            // Otherwise, it regens at normal rate when let go early.
            else
            { 
                currEnergy += energyRegen * Time.deltaTime;
                currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);
            }
        }

        string statusUpdate = "Ready";
        if (isBlocking) statusUpdate = "Blocking";
        else if (noEnergy) statusUpdate = "No Energy";

        statusText.text = $"Energy: {currEnergy:F1}/{maxEnergy:F1} - Status: {statusUpdate}";
    }

    private void StartBlock()
    {
        if (!isBlocking && !noEnergy && currEnergy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
            Debug.Log("Blocking.");
        }
        else if (noEnergy || currEnergy == 0) Debug.Log("Out of energy.");
    }

    private void EndBlock(bool autoRelease)
    {
        if (isBlocking)
        {
            isBlocking = false;

            if (autoRelease) Debug.Log("Block Released early.");
        }
    }
}