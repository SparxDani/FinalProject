using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy
{
    private Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();

        currentPosition = generationCoordinates;

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
        Vector3Int newDirection;

        do
        {
            newDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
        } while (newDirection == -currentDirection);

        currentDirection = newDirection;
    }
}
