using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GridSystem : MonoBehaviour
{
    [SerializeField] public float nodeOffset = 1f;
    [SerializeField] private TextAsset mapFile;
    [SerializeField] public Vector3Int startCoordinate = Vector3Int.zero;

    public Node[,] nodes;
    public int width;
    public int length;

    void Awake()
    {
        LoadMapFromFile();
        CreateGrid();
        ConnectNodes();
    }

    void LoadMapFromFile()
    {
        string[] lines = mapFile.text.Trim().Split('\n');
        length = lines.Length;
        width = lines[0].Split(',').Length;
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < length)
        {
            Node node = nodes[x, z];
            if (node != null)
            {
                return new Vector3(node.position.x, 0, node.position.z);

            }
        }
        Debug.LogError("Coordenadas de cuadrícula inválidas o nodo nulo.");

        return Vector3.zero;
    }

    void CreateGrid()
    {
        nodes = new Node[width, length];

        string[] lines = mapFile.text.Trim().Split('\n');

        for (int z = 0; z < length; z++)
        {
            string[] lineValues = lines[z].Trim().Split(',');

            for (int x = 0; x < width; x++)
            {
                string value = lineValues[x].Trim();
                Vector3Int position = startCoordinate + new Vector3Int(Mathf.RoundToInt(x * nodeOffset), 0, Mathf.RoundToInt(z * nodeOffset));

                bool isWalkable = (value == "0");

                nodes[x, z] = new Node(position, isWalkable);
            }
        }
    }

    void ConnectNodes()
    {
        Vector3Int[] directions = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Node currentNode = nodes[x, z];

                if (currentNode == null || !currentNode.isWalkable) continue;

                foreach (var direction in directions)
                {
                    int neighborX = x + direction.x;
                    int neighborZ = z + direction.z;

                    if (neighborX >= 0 && neighborX < width && neighborZ >= 0 && neighborZ < length)
                    {
                        Node neighborNode = nodes[neighborX, neighborZ];
                        if (neighborNode != null && neighborNode.isWalkable)
                        {
                            currentNode.ady.Add(neighborNode);
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Node node = nodes[x, z];
                if (node == null) continue;

                Gizmos.color = node.isWalkable ? Color.green : Color.red;
                Vector3 nodePosition = new Vector3(node.position.x, 0, node.position.z);

                Gizmos.DrawWireCube(nodePosition, new Vector3(nodeOffset, 0.1f, nodeOffset));
            }
        }
    }

    

    public class Node
    {
        public Vector3Int position;
        public List<Node> ady = new List<Node>();
        public bool isWalkable;

        public Node(Vector3Int position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }
    }
}
