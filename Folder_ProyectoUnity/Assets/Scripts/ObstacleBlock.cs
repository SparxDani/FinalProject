using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBlock : MonoBehaviour
{
    private Collider obstacleCollider;

    private void Awake()
    {
        obstacleCollider = GetComponent<Collider>();

        if (obstacleCollider == null)
        {
            Debug.LogError("No se encontró un Collider en el Obstacle.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Obstacle ha sido colisionado por: " + other.gameObject.name);
    }
}
