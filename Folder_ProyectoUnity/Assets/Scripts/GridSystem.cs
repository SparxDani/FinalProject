using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    [SerializeField] public int width = 10;
    [SerializeField] public int length = 10;
    [SerializeField] public float nodeOffset = 1f;

    public Node[,] nodes;

    void Awake()
    {
        CreateGrid();
        ConnectNodes();
    }

    void CreateGrid()
    {
        nodes = new Node[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3Int position = new Vector3Int(Mathf.RoundToInt(x * nodeOffset), 0, Mathf.RoundToInt(z * nodeOffset));
                nodes[x, z] = new Node(position);
            }
        }
    }

    void ConnectNodes()
    {
        Vector3Int[] direcciones = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Node nodoActual = nodes[x, z];
                for (int i = 0; i < direcciones.Length; i++)
                {
                    int vecinoX = x + direcciones[i].x;
                    int vecinoZ = z + direcciones[i].z;
                    if (vecinoX >= 0 && vecinoX < width && vecinoZ >= 0 && vecinoZ < length)
                    {
                        nodoActual.ady.Add(nodes[vecinoX, vecinoZ]);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        Gizmos.color = Color.green;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 nodePosition = new Vector3(nodes[x, z].position.x, 0, nodes[x, z].position.z);
                Gizmos.DrawWireCube(nodePosition, new Vector3(nodeOffset, 0.1f, nodeOffset));
            }
        }
    }

    public class Node
    {
        public Vector3Int position;
        public List<Node> ady = new List<Node>();

        public Node(Vector3Int position)
        {
            this.position = position;
        }
    }
}
