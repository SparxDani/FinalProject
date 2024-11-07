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

    public void ToggleIceBlocks()
    {
        if (CountIceBlocksInFront() > 0)
        {
            DestroyIceBlocksInFront();
            Debug.Log("Destruyendo bloques de hielo al frente.");
        }
        else
        {
            CreateIceBlocksInFront();
            Debug.Log("Creando bloques de hielo al frente.");
        }
    }

    private int CountIceBlocksInFront()
    {
        Vector3 direction = transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, rayDistance, detectionLayer);
        int iceBlockCount = 0;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("IceBlock"))
            {
                iceBlockCount++;
            }
            else if (hit.collider.CompareTag("Limit"))
            {
                break;
            }
        }

        return iceBlockCount;
    }

    private void CreateIceBlocksInFront()
    {
        Vector3 direction = transform.forward;
        Vector3 position = transform.position;

        for (int i = 0; i < rayDistance; i++)
        {
            position += direction;

            if (!Physics.Raycast(position, Vector3.down, 1f, detectionLayer) ||
                Physics.Raycast(position, direction, 1f, detectionLayer, QueryTriggerInteraction.Collide))
            {
                break;
            }

            GameObject iceBlock = Instantiate(Resources.Load<GameObject>("IceBlockPrefab"), position, Quaternion.identity);
            iceBlock.tag = "IceBlock";
        }
    }

    private void DestroyIceBlocksInFront()
    {
        Vector3 direction = transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, rayDistance, detectionLayer);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("IceBlock"))
            {
                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Limit"))
            {
                break;
            }
        }
    }
}
