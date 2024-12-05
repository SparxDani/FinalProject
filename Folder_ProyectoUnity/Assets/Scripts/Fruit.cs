using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public static event Action<int> OnFruitCollected;
    public Vector3Int generationCoordinates;
    public int points = 10;
    public FruitManager FruitManager;
    protected bool isCollected = false;
    public GridSystem gridSystem;
    public AudioSource audioSource;
    public AudioClip collectedFruitSound;

    public bool IsCollected => isCollected;

    protected virtual void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not found!");
            return;
        }

        Vector3 worldPosition = gridSystem.GetWorldPosition(generationCoordinates.x, generationCoordinates.z);
        transform.position = new Vector3(worldPosition.x, 0, worldPosition.z);
    }

    public virtual void Collect()
    {
        if (!isCollected)
        {
            PlaySound(collectedFruitSound);
            isCollected = true;
            OnFruitCollected?.Invoke(points);
            FruitManager.OnFruitCollected(this);
            Destroy(gameObject);
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
