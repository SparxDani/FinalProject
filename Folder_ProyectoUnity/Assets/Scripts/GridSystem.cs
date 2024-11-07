using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GridSystem : MonoBehaviour
{
    [SerializeField] public float nodeOffset = 1f;
    [SerializeField] private TextAsset mapFile;
    [SerializeField] public Vector3Int startCoordinate = Vector3Int.zero;
    [SerializeField] public GameObject iceBlockPrefab;

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
        Node node = GetNode(x, z);
        if (node != null)
        {
            return new Vector3(node.position.x, 0, node.position.z);
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

                bool isWalkable = value == "0";
                bool isIceBlock = value == "H";

                nodes[x, z] = new Node(position, isWalkable);

                if (isIceBlock)
                {
                    nodes[x, z].isIceBlock = true;
                    nodes[x, z].isWalkable = false;
                    nodes[x, z].iceBlockInstance = Instantiate(iceBlockPrefab, new Vector3(position.x, 0, position.z), Quaternion.identity);
                }
            }
        }
    }
    private Node GetNode(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < length)
        {
            return nodes[x, z];
        }
        return null;
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

                Gizmos.color = node.isWalkable ? Color.green : (node.isIceBlock ? Color.cyan : Color.red);
                Vector3 nodePosition = new Vector3(node.position.x, 0, node.position.z);

                Gizmos.DrawWireCube(nodePosition, new Vector3(nodeOffset, 0.1f, nodeOffset));
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

                for (int i = 0; i < directions.Length; i++)
                {
                    int neighborX = x + directions[i].x;
                    int neighborZ = z + directions[i].z;

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

    public class Node
    {
        public Vector3Int position;
        public List<Node> ady = new List<Node>();
        public bool isWalkable;
        public bool isIceBlock;
        public GameObject iceBlockInstance;

        public Node(Vector3Int position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
            this.isIceBlock = false;
            this.iceBlockInstance = null;
        }
    }
}
