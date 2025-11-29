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

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        playerAnimator = player.GetComponent<Animator>();

        // Saves the current node name the player is on.
        // This ensures the player goes back to their proper position on the map after a battle.
        string nodeName = GameManager.Instance.GetCurrentNodeName();
        Node foundNode = GameObject.Find(nodeName)?.GetComponent<Node>();

        if (foundNode != null)
            currentNode = foundNode;

        player.transform.position = currentNode.transform.position;
    }

    // Moves across the map to the selected node.
    // Once on a node, find its name and sets it as the current node.
    // If the node is an enemy node, swap to battle scene else NEED TO IMPLEMENT SHOP/HEAL node.
    void Update()
    {
        if (isMoving)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            playerAnimator.SetBool("isWalking", true);

            if (Vector3.Distance(player.transform.position, targetPosition) < 0.01f)
            {
                player.transform.position = targetPosition;
                isMoving = false;
                playerAnimator.SetBool("isWalking", false);
                currentNode.isVisited = true;

                if (currentNode.hasEnemies == true)
                {
                    GameManager.Instance.SetCurrentNode(currentNode);
                    GameManager.Instance.ChangeScene(2);
                }
                else if (currentNode.nodeType == NodeType.SafeZone && currentNode.hasEnemies == false)
                {
                    Animator monkAnimator = currentNode.GetComponentInChildren<Animator>();
                    if (monkAnimator != null)
                    {
                        monkAnimator.SetTrigger("Heal");
                    }
                    StartCoroutine(PlayHealSequence());
                }
            }
        }
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

    private IEnumerator PlayHealSequence()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySound(SoundName.cure);

        GameObject healEffect = Instantiate(healPrefab, targetPosition + new Vector3(0, -.1f, 0), Quaternion.identity);
        Destroy(healEffect, 1f);

        PlayerLogic playerLogic = player.GetComponent<PlayerLogic>();
        if (playerLogic != null)
        {
            playerLogic.HealHealth(3f);
        }
    }

}
