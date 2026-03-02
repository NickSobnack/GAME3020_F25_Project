using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum NodeType
{
    None,
    Healer,
    Shop,
    Enemy,
    Boss
}

public class Node : MonoBehaviour
{
    public List<Node> nextNodes = new List<Node>();

    [Header("Visual Component")]
    public NodeType nodeType;
    public GameObject monasteryPrefab, shopPrefab, towerPrefab, castlePrefab, monkPrefab, shopkeeperPrefab;
    public Vector3 heightOffset = new Vector3(0, 1, 0);
    public Vector3 npcOffset = new Vector3(1, 0, 0);
    public bool hasEnemies, isVisited, isStartingNode;
    public bool IsSafeNode => nodeType == NodeType.Healer || nodeType == NodeType.Shop;

    // Setup the node visual depending on its type = safe, enemy or boss encounter.
    // Automatically assign nodes with enemies for scene transition based on NodeType.
    void Start()
    {
        if (isStartingNode) return;

        if (nodeType == NodeType.Enemy || nodeType == NodeType.Boss)
        {
            hasEnemies = true;
        }
        else if (IsSafeNode)  
        {
            hasEnemies = false;
            if (nodeType == NodeType.Healer)
                Instantiate(monkPrefab, transform.position + npcOffset, Quaternion.identity, transform);
            else if (nodeType == NodeType.Shop)
                Instantiate(shopkeeperPrefab, transform.position + npcOffset, Quaternion.identity, transform);
        }

        GameObject selectedPrefab = null;
        switch (nodeType)
        {
            case NodeType.Healer:
                selectedPrefab = monasteryPrefab;
                break;
            case NodeType.Shop:
                selectedPrefab = shopPrefab;
                break;
            case NodeType.Enemy:
                selectedPrefab = towerPrefab;
                break;
            case NodeType.Boss:
                selectedPrefab = castlePrefab;
                break;
        }

        if (selectedPrefab != null)
        {
            GameObject buildingInstance = Instantiate(selectedPrefab, transform.position, Quaternion.identity, transform);
            buildingInstance.transform.localPosition += heightOffset;
        }
    }

    // On mouse click, move the player to the selected node via MapLogic.
    private void OnMouseDown()
    {
        var mover = GameObject.FindFirstObjectByType<MapLogic>();
        mover.MoveTo(this);
    }

    // Debug gizmo to visualize node and paths.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.white;
        foreach (Node next in nextNodes)
        {
            if (next != null)
            {
                Gizmos.DrawLine(transform.position, next.transform.position);
            }
        }
    }
}
