using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public Vector3Int generationCoordinates;
    [SerializeField] public float moveSpeed = 2f;

    protected GridSystem gridSystem;
    protected Vector3Int currentDirection = Vector3Int.forward;
    protected Vector3Int currentPosition;

    protected virtual void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();

        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not found!");
            return;
        }

        int x = generationCoordinates.x;
        int z = generationCoordinates.z;

        if (x < 0 || x >= gridSystem.width || z < 0 || z >= gridSystem.length)
        {
            Debug.LogError($"Coordenadas fuera de los límites del grid: ({x}, {z})");
            return;
        }

        GridSystem.Node node = gridSystem.GetNode(x, z);
        if (node == null || !node.isWalkable)
        {
            Debug.LogError($"No se encontró un nodo válido en las coordenadas ({x}, {z}) o el nodo no es caminable.");
            return;
        }

        transform.position = new Vector3(node.position.x, 0, node.position.z);
        currentPosition = new Vector3Int(x, 0, z); 
    }

    public abstract void Move();
}
