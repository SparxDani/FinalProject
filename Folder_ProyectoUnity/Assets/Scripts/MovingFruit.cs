using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingFruit : Fruit
{
    private GridSystem.Node targetNode;
    private bool isMoving = false;
    public float speed = 3f;

    protected override void Start()
    {
        base.Start();
        gridSystem = FindObjectOfType<GridSystem>();

        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not found!");
            return;
        }

        StartCoroutine(MoveRandomly());
    }

    private IEnumerator MoveRandomly()
    {
        while (!isCollected)
        {
            if (!isMoving)
            {
                GridSystem.Node currentNode = GetNodeAtPosition(transform.position);

                if (currentNode != null)
                {
                    List<GridSystem.Node> walkableNodes = currentNode.ady.FindAll(node => node.isWalkable);

                    if (walkableNodes.Count > 0)
                    {
                        targetNode = walkableNodes[Random.Range(0, walkableNodes.Count)];
                        isMoving = true;
                    }
                }
                else
                {
                    Debug.LogWarning("Nodo actual no encontrado.");
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        if (isMoving && targetNode != null)
        {
            Vector3 targetPosition = new Vector3(targetNode.position.x, 0, targetNode.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public override void Collect()
    {
        base.Collect();
        Debug.Log("MovingFruit collected!");
    }

    private GridSystem.Node GetNodeAtPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt((position.x - gridSystem.startCoordinate.x) / gridSystem.nodeOffset);
        int z = Mathf.RoundToInt((position.z - gridSystem.startCoordinate.z) / gridSystem.nodeOffset);

        if (x >= 0 && x < gridSystem.width && z >= 0 && z < gridSystem.length)
        {
            return gridSystem.nodes[x, z];
        }

        return null;
    }
}
