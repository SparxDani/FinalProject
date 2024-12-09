using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class GridSystem : MonoBehaviour
{
    [SerializeField] public float nodeOffset = 1f;
    [SerializeField] public float actionDelay = 0.5f;
    [SerializeField] private TextAsset mapFile;
    [SerializeField] public Vector3Int startCoordinate = Vector3Int.zero;
    [SerializeField] public GameObject iceBlockPrefab;
    [SerializeField] public GameObject[] obstaclePrefabs;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip createIceSound;
    [SerializeField] private AudioClip destroyIceSound;

    [SerializeField] private AudioSource audioSource;

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

    public Node GetNode(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < length)
        {
            return nodes[x, z];
        }
        return null;
    }
    public void ToggleIceBlocks(Vector3Int playerPosition, Vector3Int direction)
    {
        StopAllCoroutines();
        StartCoroutine(ToggleIceBlocksCoroutine(playerPosition, direction));
    }
    private IEnumerator ToggleIceBlocksCoroutine(Vector3Int playerPosition, Vector3Int direction)
    {
        Vector3Int currentPosition = playerPosition + direction;
        List<Node> affectedNodes = new List<Node>();

        bool playedDestroySound = false; 

        while (true)
        {
            int x = Mathf.RoundToInt((currentPosition.x - startCoordinate.x) / nodeOffset);
            int z = Mathf.RoundToInt((currentPosition.z - startCoordinate.z) / nodeOffset);

            if (x < 0 || x >= width || z < 0 || z >= length)
                break;

            Node node = nodes[x, z];

            if (node.enemy != null)
            {
                Debug.Log("Creación de bloques de hielo detenida por la presencia de un enemigo.");
                break;
            }

            if (node == null || (!node.isIceBlock))
                break;

            affectedNodes.Add(node);
            currentPosition += direction;
        }

        for (int i = 0; i < affectedNodes.Count; i++)
        {
            Node node = affectedNodes[i];
            if (node.isIceBlock)
            {
                if (!playedDestroySound)
                {
                    PlaySound(destroyIceSound);
                    playedDestroySound = true;
                }

                Destroy(node.iceBlockInstance);
                node.isIceBlock = false;
                node.isWalkable = true;
                yield return new WaitForSeconds(0.2f);
            }
        }

        if (affectedNodes.Count > 0)
            yield break;

        currentPosition = playerPosition + direction;
        bool playedCreateSound = false;

        while (true)
        {
            int x = Mathf.RoundToInt((currentPosition.x - startCoordinate.x) / nodeOffset);
            int z = Mathf.RoundToInt((currentPosition.z - startCoordinate.z) / nodeOffset);

            if (x < 0 || x >= width || z < 0 || z >= length)
                break;

            Node node = nodes[x, z];

            if (node.enemy != null)
            {
                Debug.Log("Creación de bloques de hielo detenida por la presencia de un enemigo.");
                break;
            }

            if (node == null || (!node.isWalkable && !node.isIceBlock))
                break;

            if (node.isWalkable)
            {
                if (!playedCreateSound)
                {
                    PlaySound(createIceSound);
                    playedCreateSound = true;
                }

                node.iceBlockInstance = Instantiate(iceBlockPrefab, new Vector3(node.position.x, 0, node.position.z), Quaternion.identity);
                node.isIceBlock = true;
                node.isWalkable = false;
                yield return new WaitForSeconds(actionDelay);
            }

            currentPosition += direction;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
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
        public GameObject obstacleInstance;
        public Collider nodeCollider;
        public Enemy enemy;

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
