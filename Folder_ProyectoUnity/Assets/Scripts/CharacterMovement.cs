using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public GridSystem gridSystem;
    public float speed = 5f;
    private GridSystem.Node actualNode;
    private GridSystem.Node targetNode;
    private bool inMoving;
    private Vector2 inputDirection;

    void Start()
    {
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not assigned in CharacterMovement.");
            return;
        }

        actualNode = gridSystem.nodes[0, 0];
        targetNode = actualNode;
        transform.position = new Vector3(actualNode.position.x, 0, actualNode.position.z);
    }

    void Update()
    {
        if (!inMoving && inputDirection != Vector2.zero)
        {
            Vector3Int direction = GetValidDirection(inputDirection);
            if (direction != Vector3Int.zero)
            {
                Move(direction);
            }
        }

        if (inMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetNode.position.x, 0, targetNode.position.z), speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, new Vector3(targetNode.position.x, 0, targetNode.position.z)) < 0.1f)
            {
                transform.position = new Vector3(targetNode.position.x, 0, targetNode.position.z);
                actualNode = targetNode;
                inMoving = false;
            }
        }
    }

    public void OnMovePerformed(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    public void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputDirection = Vector2.zero;
    }

    void Move(Vector3Int direction)
    {
        int newX = Mathf.RoundToInt((actualNode.position.x + direction.x * gridSystem.nodeOffset) / gridSystem.nodeOffset);
        int newZ = Mathf.RoundToInt((actualNode.position.z + direction.z * gridSystem.nodeOffset) / gridSystem.nodeOffset);

        if (newX >= 0 && newX < gridSystem.width && newZ >= 0 && newZ < gridSystem.length)
        {
            targetNode = gridSystem.nodes[newX, newZ];
            inMoving = true;
        }
    }

    Vector3Int GetValidDirection(Vector2 inputDir)
    {
        if (Mathf.Abs(inputDir.x) > Mathf.Abs(inputDir.y))
        {
            return new Vector3Int(Mathf.RoundToInt(Mathf.Sign(inputDir.x)), 0, 0);
        }
        else if (Mathf.Abs(inputDir.y) > Mathf.Abs(inputDir.x))
        {
            return new Vector3Int(0, 0, Mathf.RoundToInt(Mathf.Sign(inputDir.y)));
        }

        return Vector3Int.zero;
    }
}
