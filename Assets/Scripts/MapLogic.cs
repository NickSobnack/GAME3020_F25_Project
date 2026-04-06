using UnityEngine;
using System.Collections;

public class MapLogic : MonoBehaviour
{
    [Header("Movement Settings")]
    public Node currentNode;
    public float moveSpeed = 3f;
    [SerializeField] private GameObject healPrefab;

    [Header("Player Settings")]
    public GameObject player;
    private Animator playerAnimator;
    public GameObject shopPanel;
    public ShopLogic shopLogic;

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        playerAnimator = player.GetComponent<Animator>();
        shopLogic = shopPanel.GetComponent<ShopLogic>();

        string nodeName = GameManager.Instance.GetCurrentNodeName();
        Node foundNode = GameObject.Find(nodeName)?.GetComponent<Node>();

        if (foundNode != null)
            currentNode = foundNode;

        player.transform.position = currentNode.transform.position;
    }

    void Update()
    {
        OnClickNode();

        if (!isMoving) return;

        MovePlayer();

        if (HasReachedTarget())
            OnNodeArrival();
    }

    private void OnClickNode()
    {
        if (isMoving || shopPanel.activeSelf || !Input.GetMouseButtonDown(0)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null) return;

        Node node = hit.collider.GetComponent<Node>();
        if (node == null)
            node = hit.collider.GetComponentInParent<Node>();

        if (node != null)
            MoveTo(node);
    }

    private void MovePlayer()
    {
        player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);
        playerAnimator.SetBool("isWalking", true);
    }

    private bool HasReachedTarget()
    {
        return Vector3.Distance(player.transform.position, targetPosition) < 0.01f;
    }

    private void OnNodeArrival()
    {
        player.transform.position = targetPosition;
        isMoving = false;
        playerAnimator.SetBool("isWalking", false);
        currentNode.isVisited = true;

        if (currentNode.hasEnemies)
            EnterBattle();
        else if (currentNode.IsSafeNode)
            EnterSafeNode();
    }

    private void EnterBattle()
    {
        GameManager.Instance.SetCurrentNode(currentNode);
        GameManager.Instance.ChangeScene(2);
    }

    private void EnterSafeNode()
    {
        if (currentNode.nodeType == NodeType.Healer)
            RecoverHealth();
        else if (currentNode.nodeType == NodeType.Shop)
            OpenShop();
    }

    private void RecoverHealth()
    {
        Animator monkAnimator = currentNode.GetComponentInChildren<Animator>();
        if (monkAnimator != null)
            monkAnimator.SetTrigger("Heal");

        StartCoroutine(PlayHealSequence());

        float healAmount = 3f;
        GameManager.Instance.playerHealth = Mathf.Clamp (GameManager.Instance.playerHealth + healAmount, 0f,
                                                GameManager.Instance.playerMaxHealth);

        Debug.Log($"Player healed for {healAmount}. Current HP: {GameManager.Instance.playerHealth}");
    }

    private void OpenShop()
    {
        BackgroundBlurManager.Instance.RegisterPanelOpened();
        shopPanel.SetActive(true);
        shopLogic.ResetShop();
    }

    private IEnumerator PlayHealSequence()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySound(SoundName.cure);

        GameObject healEffect = Instantiate(healPrefab, targetPosition + new Vector3(0, -.1f, 0), Quaternion.identity);
        Destroy(healEffect, 1f);
    }


    public void MoveTo(Node nextNode)
    {
        if (currentNode.nextNodes.Contains(nextNode) && !nextNode.isVisited)
        {
            currentNode = nextNode;
            targetPosition = nextNode.transform.position;
            isMoving = true;
        }
    }
}
