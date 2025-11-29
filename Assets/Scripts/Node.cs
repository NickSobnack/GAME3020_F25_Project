using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum NodeType
{
    SafeZone,
    Enemy,
    Boss
}

public class Node : MonoBehaviour
{
    public List<Node> nextNodes = new List<Node>();

    [Header("Visual Component")]
    public NodeType nodeType;
    public Vector3 heightOffset = new Vector3(0, 1f, 0);
    public GameObject monasteryPrefab, towerPrefab, castlePrefab;
    public GameObject monkPrefab;
    public Vector3 priestOffset = new Vector3(0, 0, 0);
    public bool hasEnemies, isVisited, isStartingNode;

    // Setup the node visual depending on its type = safe, enemy or boss encounter.
    // Automatically assign nodes with enemies for scene transition based on NodeTyp,
    void Start()
    {
        if (isStartingNode) return;

        if (nodeType == NodeType.Enemy || nodeType == NodeType.Boss)
        {
            hasEnemies = true;
        }
        else if (nodeType == NodeType.SafeZone)
        {
            hasEnemies = false;
            GameObject priestInstance = Instantiate(monkPrefab, transform.position + priestOffset,Quaternion.identity, transform);
        }

        GameObject selectedPrefab = null;
        switch (nodeType)
        {
            case NodeType.SafeZone:
                selectedPrefab = monasteryPrefab;
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
