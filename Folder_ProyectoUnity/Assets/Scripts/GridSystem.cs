using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class GridSystem : MonoBehaviour
{
    [SerializeField] public float nodeOffset = 1f;
    [SerializeField] private TextAsset mapFile;
    [SerializeField] public Vector3Int startCoordinate = Vector3Int.zero;
    [SerializeField] public GameObject iceBlockPrefab;
    [SerializeField] public GameObject[] obstaclePrefabs; 

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

                if (value == "0")
                {
                    nodes[x, z] = new Node(position, true);
                }
                else if (value == "H")
                {
                    nodes[x, z] = new Node(position, false);
                    nodes[x, z].isIceBlock = true;
                    nodes[x, z].iceBlockInstance = Instantiate(iceBlockPrefab, new Vector3(position.x, 0, position.z), Quaternion.identity);
                }
                else if (int.TryParse(value, out int obstacleIndex) && obstacleIndex >= 1 && obstacleIndex <= obstaclePrefabs.Length)
                {
                    nodes[x, z] = new Node(position, false);
                    nodes[x, z].obstacleInstance = Instantiate(obstaclePrefabs[obstacleIndex - 1], new Vector3(position.x, 0, position.z), Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning($"Valor no reconocido en la posición ({x},{z}): {value}");
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
        public GameObject obstacleInstance;
        public Collider nodeCollider;

        public Node(Vector3Int position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
            this.isIceBlock = false;
            this.iceBlockInstance = null;
            this.obstacleInstance = null;

            if (iceBlockInstance != null)
            {
                nodeCollider = iceBlockInstance.GetComponent<Collider>();
            }
            else if (obstacleInstance != null)
            {
                nodeCollider = obstacleInstance.GetComponent<Collider>();
            }
        }
    }

}
