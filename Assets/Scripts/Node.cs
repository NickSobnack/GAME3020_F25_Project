using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> nextNodes = new List<Node>();
    public bool hasEnemies;
    public bool isVisited;

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

        Handles.Label(transform.position + Vector3.up * 0.3f, name);
    }
}