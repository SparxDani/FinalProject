using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    private Collider blockCollider;

    private void Awake()
    {
        blockCollider = GetComponent<Collider>();

        if (blockCollider == null)
        {
            Debug.LogError("No se encontró un Collider en el IceBlock.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("IceBlock ha sido colisionado por: " + other.gameObject.name);
    }
    public void DestroyBlock()
    {
        Destroy(gameObject);
    }

    
}
