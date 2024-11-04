using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public Vector3Int generationCoordinates;
    public int points = 10;
    public FruitManager FruitManager;
    protected bool isCollected = false;
    public GridSystem gridSystem;

    public bool IsCollected
    {
        get { return isCollected; }
    }

    protected virtual void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            Debug.LogError("¡GridSystem no encontrado!");
            return;
        }

        Vector3 worldPosition = gridSystem.GetWorldPosition(generationCoordinates.x, generationCoordinates.z);
        transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);

        Debug.Log("Fruta generada en la posición de grid: " + generationCoordinates + " con posición en el mundo: " + transform.position);
    }


    public virtual void Collect()
    {
        if (!isCollected)
        {
            isCollected = true;
            FruitManager.OnFruitCollected(this);
            Destroy(gameObject);
            Debug.Log("Fruit collected!");
        }
    }
}
