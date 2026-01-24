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

    [Header("Energy System")]

    private PlayerLogic playerLogic;

    [SerializeField] private float energyRegen = 1f;
    [SerializeField] private float blockCost = 1f;
    [SerializeField] private float attackCost = 3f;
    [SerializeField] private float attackCooldown = 3f;
    private float lastAttackTime;


    private float currHoldTimer;
    private bool isAttacking, isBlocking, noEnergy;

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

    [Header("Enemy")]
    [SerializeField] private List<EnemyBase> enemies = new(2); 
    [SerializeField] private GameObject targetPointerPrefab;
    [SerializeField] public float playerDmg = 3f;
    public EnemyBase selectedTarget;
    private int selectedIndex = 0;

    public bool inAction { get; private set; }
    public bool isDeflecting { get; private set; }

    private void Awake()
    {
        noEnergy = false;
        playerLogic = GetComponent<PlayerLogic>();
    }

    // Populate enemy list at the start of the scene.
    void Start()
    {
        selectedIndex = 0;
    }

    private void Update()
    {
        float currEnergy = playerLogic.energy;
        float maxEnergy = playerLogic.maxEnergy;

        if (isBlocking)
        {
            currHoldTimer += Time.deltaTime;
            currEnergy -= blockCost * Time.deltaTime;
            currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);

            if (currEnergy == 0)
            {
                noEnergy = true;
                StopBlocking();
            }
        }
        else if (!isAttacking)
        {
            if (noEnergy)
            {
                currEnergy += (energyRegen * 0.5f) * Time.deltaTime;
                if (currEnergy >= maxEnergy)
                {
                    currEnergy = maxEnergy;
                    noEnergy = false;
                }
            }
            else
            {
                currEnergy += energyRegen * Time.deltaTime;
                currEnergy = Mathf.Clamp(currEnergy, 0, maxEnergy);
            }
        }

        // push back into PlayerLogic
        playerLogic.energy = currEnergy;
    }

    // Attack the selected target if not blocking, has enough energy, and not already attacking.
    // If all enemies are defeated, trigger win message and load next scene.
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            bool canAttack = Time.time >= lastAttackTime + attackCooldown;

            if (!isBlocking && playerLogic.energy >= attackCost && !isAttacking && selectedTarget != null && canAttack)
            {
                isAttacking = true; 
                lastAttackTime = Time.time; 
                playerLogic.energy -= attackCost;
                originalPos = transform.position;
                StartCoroutine(AttackSequence(selectedTarget));
            }
        }
    }


    public void Deflect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!isBlocking && playerLogic.energy >= attackCost && !isAttacking && selectedTarget != null)
            {
                playerLogic.energy -= attackCost;
                StartCoroutine(DeflectSequence(selectedTarget));
            }
        }
    }

    // Block function so that when block key is held and player has energy, the block animation plays.
    public void StartBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!isBlocking && !noEnergy && playerLogic.energy > 0)
            {
                isBlocking = true;
                currHoldTimer = 0f;
                currBlockTime = Time.time;
                playerAnimator.SetBool("isBlocking", true);
            }
        }
    }

    // Ends blocking when block key is released.
    public void EndBlock(InputAction.CallbackContext context)
    {
        if (context.canceled)
            StopBlocking();
    }

    // Stops blocking animation and state.
    private void StopBlocking()
    {
        if (isBlocking)
        {
            isBlocking = false;
            playerAnimator.SetBool("isBlocking", false);
        }
    }

    // Cycles through available enemies as target when tab is pressed.
    public void SelectTarget(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    // Animation event triggers after attack animation finishes, to prevent attack spam.
    public void AttackFinished()
    {
        isAttacking = false;
    }

    // Coroutine to handle moving to enemy using lerp, play attack animation and deal damage then return to og position.
    private IEnumerator AttackSequence(EnemyBase target)
    {
        SetInAction(true);

        Vector3 direction = (target.transform.position - transform.position).normalized;
        Vector3 attackPos = target.transform.position - direction * distanceToEnemy;

        yield return MoveToPosition(attackPos);

        playerAnimator.SetTrigger("Attack");
        AudioManager.Instance.PlaySound(SoundName.sword);
        yield return new WaitForSeconds(0.5f);

        target.TakeDamage(playerDmg);
        Debug.Log($"Attack dealt {playerDmg} damage to {target.name}");
        CheckAllEnemiesDefeated();

        yield return new WaitForSeconds(returnDelay);
        yield return MoveToPosition(originalPos);

        isAttacking = false;
        SetInAction(false); 
    }

    private IEnumerator DeflectSequence(EnemyBase target)
    {
        isDeflecting = true;
        isAttacking = true;
        SetInAction(true);

        playerAnimator.SetTrigger("Deflect");

        yield return new WaitForSeconds(0.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f); 
        foreach (var hit in hits)
        {
            IDeflect deflectable = hit.GetComponent<IDeflect>();
            if (deflectable != null)
            {
                deflectable.Deflect(transform.right);
            }
        }
        isDeflecting = false;
        yield return new WaitForSeconds(returnDelay);

        isAttacking = false;
        SetInAction(false);
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

    public void RegisterEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    // Check if all enemies are defeated, then play perfect vfx and load next scene after delay.
    private void CheckAllEnemiesDefeated()
    {
        enemies.RemoveAll(e => e == null || e.health <= 0); if (enemies.Count > 0) return;

        Instantiate(perfectVfx, gameStatusVfxPoint.position, Quaternion.identity);
        AudioManager.Instance.PlayMusic(MusicName.victory, false);
        GameManager.Instance.DelayLoadScene(1, 3f);
    }

    public void SetInAction(bool busy)
    {
        inAction = busy;
    }

    public void MobileAttack()
    {
        bool canAttack = Time.time >= lastAttackTime + attackCooldown;

        if (!isBlocking &&
            playerLogic.energy >= attackCost &&
            !isAttacking &&
            selectedTarget != null &&
            canAttack)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            playerLogic.energy -= attackCost;
            originalPos = transform.position;
            StartCoroutine(AttackSequence(selectedTarget));
        }
    }

    public void MobileStartBlock()
    {
        if (!isBlocking && !noEnergy && playerLogic.energy > 0)
        {
            isBlocking = true;
            currHoldTimer = 0f;
            currBlockTime = Time.time;
            playerAnimator.SetBool("isBlocking", true);
        }
    }

    public void MobileEndBlock()
    {
        StopBlocking();
    }

    public void SelectEnemy(EnemyBase enemy)
    {
        if (enemy == null || enemy.health <= 0)
            return;

        if (selectedTarget != null)
            selectedTarget.SetSelected(false);

        selectedTarget = enemy;
        selectedTarget.SetSelected(true);
    }


}