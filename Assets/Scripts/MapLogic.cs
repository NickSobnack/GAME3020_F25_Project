using UnityEngine;

public class MapLogic : MonoBehaviour
{
    [Header("Movement Settings")]
    public Node currentNode;
    public float moveSpeed = 3f;

    [Header("Player Settings")]
    public GameObject player;
    private Animator playerAnimator;

    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        playerAnimator = player.GetComponent<Animator>();


        if (GameManager.Instance.GetCurrentNode() != null)
        {
            currentNode = GameManager.Instance.GetCurrentNode();
            player.transform.position = currentNode.transform.position;
        }
        else
        {
            player.transform.position = currentNode.transform.position;
        }

    }

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

                if (currentNode.hasEnemies)
                {
                    Debug.Log("Enemies here!");
                    GameManager.Instance.SetCurrentNode(currentNode); 
                    GameManager.Instance.ChangeScene(2); 
                }
                else
                {
                    Debug.Log("This is a safe spot.");
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
}
