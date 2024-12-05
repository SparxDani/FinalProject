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

        Vector3 worldPosition = gridSystem.GetWorldPosition(generationCoordinates.x, generationCoordinates.z);
        transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
    }

    public abstract void Move();
}
