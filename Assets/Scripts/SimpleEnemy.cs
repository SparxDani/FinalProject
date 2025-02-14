using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleEnemy : Enemy
{
    public enum InitialDirection { Up, Down, Left, Right } // Enum para el Inspector
    public InitialDirection initialDirection; // Configurable desde el Inspector

    private Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();

        currentPosition = generationCoordinates;
        SetInitialDirection(); // Establecer dirección inicial según el Inspector

        GridSystem.Node startNode = gridSystem.GetNode(generationCoordinates.x, generationCoordinates.z);

        if (startNode != null)
        {
            transform.position = new Vector3(startNode.position.x, 0, startNode.position.z);
            targetPosition = transform.position;
        }
        else
        {
            Debug.LogError("El nodo inicial no es válido. Verifica las coordenadas de generación.");
        }
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            Move();
        }
    }

    public override void Move()
    {
        Vector3Int nextPosition = currentPosition + currentDirection;

        int x = nextPosition.x;
        int z = nextPosition.z;

        GridSystem.Node currentNode = gridSystem.GetNode(currentPosition.x, currentPosition.z);
        GridSystem.Node nextNode = gridSystem.GetNode(x, z);

        if (nextNode != null && nextNode.isWalkable && !nextNode.isIceBlock)
        {
            if (currentNode != null)
                currentNode.enemy = null;

            currentPosition = nextPosition;
            targetPosition = new Vector3(nextNode.position.x, 0, nextNode.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            nextNode.enemy = this;
        }
        else
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        Vector3Int[] possibleDirections = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
        List<Vector3Int> validDirections = new List<Vector3Int>();

        foreach (Vector3Int dir in possibleDirections)
        {
            Vector3Int nextPos = currentPosition + dir;
            GridSystem.Node nextNode = gridSystem.GetNode(nextPos.x, nextPos.z);
            if (nextNode != null && nextNode.isWalkable && !nextNode.isIceBlock && dir != -currentDirection)
            {
                validDirections.Add(dir);
            }
        }

        if (validDirections.Count > 0)
        {
            currentDirection = validDirections[Random.Range(0, validDirections.Count)];
        }
        else
        {
            currentDirection = -currentDirection; // Si no hay opciones, retrocede
        }
    }

    // Método para definir la dirección inicial desde el Inspector
    private void SetInitialDirection()
    {
        switch (initialDirection)
        {
            case InitialDirection.Up:
                currentDirection = Vector3Int.forward;
                break;
            case InitialDirection.Down:
                currentDirection = Vector3Int.back;
                break;
            case InitialDirection.Left:
                currentDirection = Vector3Int.left;
                break;
            case InitialDirection.Right:
                currentDirection = Vector3Int.right;
                break;
        }
    }
}
