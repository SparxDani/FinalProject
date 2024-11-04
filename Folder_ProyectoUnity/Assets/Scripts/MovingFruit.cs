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
            }

            yield return null;
        }
    }

    void Update()
    {
        if (isMoving && targetNode != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetNode.position.x, 0, targetNode.position.z), speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, new Vector3(targetNode.position.x, 0, targetNode.position.z)) < 0.1f)
            {
                transform.position = new Vector3(targetNode.position.x, 0, targetNode.position.z);
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
