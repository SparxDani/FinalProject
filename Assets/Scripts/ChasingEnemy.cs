//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChasingEnemy : Enemy
//{
//    private Transform player;
//    private float updateRate = 0.5f; // Frecuencia con la que calcula el movimiento

//    public void StartChasing(Transform playerTransform)
//    {
//        player = playerTransform;
//        InvokeRepeating(nameof(ChasePlayer), 0f, updateRate);
//    }

//    private void ChasePlayer()
//    {
//        if (player == null || inMoving) return;

//        Vector3Int playerPos = player.GetComponent<CharacterMovement>().GetGridPosition();
//        Vector3Int enemyPos = new Vector3Int(
//            Mathf.RoundToInt((actualNode.position.x - gridSystem.startCoordinate.x) / gridSystem.nodeOffset),
//            0,
//            Mathf.RoundToInt((actualNode.position.z - gridSystem.startCoordinate.z) / gridSystem.nodeOffset)
//        );

//        Vector3Int direction = GetBestDirection(enemyPos, playerPos);
//        int newX = enemyPos.x + direction.x;
//        int newZ = enemyPos.z + direction.z;

//        if (newX >= 0 && newX < gridSystem.width && newZ >= 0 && newZ < gridSystem.length)
//        {
//            GridSystem.Node nextNode = gridSystem.nodes[newX, newZ];
//            MoveTo(nextNode);
//        }
//    }

//    private Vector3Int GetBestDirection(Vector3Int enemyPos, Vector3Int playerPos)
//    {
//        Vector3Int direction = Vector3Int.zero;

//        if (Mathf.Abs(playerPos.x - enemyPos.x) > Mathf.Abs(playerPos.z - enemyPos.z))
//        {
//            direction.x = playerPos.x > enemyPos.x ? 1 : -1;
//        }
//        else
//        {
//            direction.z = playerPos.z > enemyPos.z ? 1 : -1;
//        }

//        return direction;
//    }
//}
