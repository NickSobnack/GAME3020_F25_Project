using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public int totalNodes = 10;
    public float spacing = 3f;
    public GameObject nodePrefab;

    private List<Node> nodes = new List<Node>();

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        if (totalNodes < 2)
        {
            Debug.LogError("Need at least 2 nodes (Start + Boss)");
            return;
        }

        Vector3 startPos = new Vector3(-8f, 0f, 0f);

        // STEP 1 — Create all nodes
        for (int i = 0; i < totalNodes; i++)
        {
            Vector3 pos;

            if (i == 0)
            {
                // Start node fixed position
                pos = startPos;
            }
            else
            {
                // Other nodes spaced relative to start
                pos = startPos + new Vector3(i * spacing, 0, 0);
            }

            GameObject obj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
            Node node = obj.GetComponent<Node>();
            nodes.Add(node);
        }

        // STEP 2 — Assign types
        nodes[0].nodeType = NodeType.SafeZone;
        nodes[0].isStartingNode = true;

        nodes[totalNodes - 1].nodeType = NodeType.Boss;

        for (int i = 1; i < totalNodes - 1; i++)
        {
            nodes[i].nodeType = (Random.value > 0.3f) ? NodeType.Enemy : NodeType.SafeZone;
        }

        // STEP 3 — Create branching connections
        for (int i = 0; i < totalNodes - 1; i++)
        {
            nodes[i].nextNodes.Add(nodes[i + 1]);

            if (i < totalNodes - 2 && Random.value < 0.3f)
            {
                nodes[i].nextNodes.Add(nodes[i + 2]);
            }
        }

        // STEP 4 — Add vertical variation
        for (int i = 1; i < nodes.Count; i++)
        {
            float yOffset = Random.Range(-1.5f, 1.5f);
            nodes[i].transform.position += new Vector3(0, yOffset, 0);
        }
    }

}
