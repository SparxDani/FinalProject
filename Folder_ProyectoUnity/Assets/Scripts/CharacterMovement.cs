using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public GridSystem gridSystem;
    public float speed = 5f;
    public float rotationSpeed = 360f;
    public Vector2Int spawnPosition;
    public float rayDistance = 20f;
    public LayerMask detectionLayer;

    private GridSystem.Node actualNode;
    private GridSystem.Node targetNode;
    private bool inMoving;
    private bool isRotating;
    private Vector2 inputDirection;
    private Quaternion targetRotation;

    void Start()
    {
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not assigned in CharacterMovement.");
            return;
        }

        int x = Mathf.Clamp(spawnPosition.x, 0, gridSystem.width - 1);
        int z = Mathf.Clamp(spawnPosition.y, 0, gridSystem.length - 1);
        actualNode = gridSystem.nodes[x, z];
        targetNode = actualNode;

        transform.position = new Vector3(actualNode.position.x, 0, actualNode.position.z);
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (!inMoving && !isRotating && inputDirection != Vector2.zero)
        {
            Vector3Int direction = GetValidDirection(inputDirection);
            if (direction != Vector3Int.zero)
            {
                Rotate(direction);
            }
        }

        if (isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
                if (inputDirection != Vector2.zero)
                {
                    Vector3Int direction = GetValidDirection(inputDirection);
                    Move(direction);
                }
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

    public void OnCreateIceBlock(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector3Int direction = GetPlayerLookDirection();

        if (direction != Vector3Int.zero)
        {
            Vector3Int startPosition = new Vector3Int(
                Mathf.RoundToInt(transform.position.x),
                0,
                Mathf.RoundToInt(transform.position.z)
            ) + direction;

            gridSystem.ToggleIceBlocks(startPosition, direction);
        }
    }

    void Rotate(Vector3Int direction)
    {
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        targetRotation = Quaternion.LookRotation(lookDirection);
        isRotating = true;
    }

    void Move(Vector3Int direction)
    {
        int newX = Mathf.RoundToInt((actualNode.position.x - gridSystem.startCoordinate.x + direction.x * gridSystem.nodeOffset) / gridSystem.nodeOffset);
        int newZ = Mathf.RoundToInt((actualNode.position.z - gridSystem.startCoordinate.z + direction.z * gridSystem.nodeOffset) / gridSystem.nodeOffset);

        if (newX >= 0 && newX < gridSystem.width && newZ >= 0 && newZ < gridSystem.length)
        {
            GridSystem.Node targetNode = gridSystem.nodes[newX, newZ];
            if (targetNode.isWalkable)
            {
                this.targetNode = targetNode;
                inMoving = true;
            }
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

    Vector3Int GetPlayerLookDirection()
    {
        Vector3 forward = transform.forward;
        Vector3Int direction = Vector3Int.zero;

        if (Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            direction.x = forward.x > 0 ? 1 : -1;
        }
        else
        {
            direction.z = forward.z > 0 ? 1 : -1;
        }

        return direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fruit"))
        {
            Debug.Log("Fruit collected!");

            Fruit fruit = other.GetComponent<Fruit>();
            if (fruit != null)
            {
                fruit.Collect();
            }
        }
    }
}
