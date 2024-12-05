using UnityEngine;
using System.Collections;

public class SimpleEnemy : Enemy
{
    private Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();
        currentPosition = generationCoordinates;
        targetPosition = transform.position;
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

        int x = Mathf.RoundToInt((nextPosition.x - gridSystem.startCoordinate.x) / gridSystem.nodeOffset);
        int z = Mathf.RoundToInt((nextPosition.z - gridSystem.startCoordinate.z) / gridSystem.nodeOffset);

        GridSystem.Node nextNode = gridSystem.GetNode(x, z);

        if (nextNode != null && nextNode.isWalkable && !nextNode.isIceBlock)
        {
            currentPosition = nextPosition;
            targetPosition = new Vector3(nextNode.position.x, 0, nextNode.position.z);
        }
        else
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        Vector3Int[] possibleDirections = { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right };
        do
        {
            currentDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];
        } while (currentDirection == -currentDirection);
    }
}
