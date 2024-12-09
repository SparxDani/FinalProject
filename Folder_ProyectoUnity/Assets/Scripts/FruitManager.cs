using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FruitList
{
    public List<Fruit> fruitList;
}

public class FruitManager : MonoBehaviour
{
    [SerializeField] public List<FruitList> fruitLists = new List<FruitList>();
    private int currentListIndex = 0;
    public UiManager Uicontroller;
    public CharacterMovement playerMovement;
    public static event Action OnVictoryAchieved;

    void Start()
    {
        if (fruitLists == null || fruitLists.Count == 0)
        {
            Debug.LogError("Fruit lists are not initialized or are empty.");
            return;
        }

        for (int i = 0; i < fruitLists.Count; i++)
        {
            List<Fruit> list = fruitLists[i]?.fruitList;
            if (list == null)
            {
                Debug.LogWarning($"Fruit list at index {i} is null. Skipping initialization.");
                continue;
            }

            bool shouldActivate = (i == 0);
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j] != null)
                {
                    list[j].gameObject.SetActive(shouldActivate);
                }
            }
        }

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<CharacterMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("CharacterMovement not found in the scene.");
            }
        }
    }

    private void OnEnable()
    {
        Fruit.OnFruitCollected += OnFruitCollected;
    }

    private void OnDisable()
    {
        Fruit.OnFruitCollected -= OnFruitCollected;
    }

    public void OnFruitCollected(Fruit fruit)
    {
        CheckCurrentListCompletion();
    }

    private void CheckCurrentListCompletion()
    {
        if (currentListIndex >= fruitLists.Count)
        {
            Debug.LogWarning("No more fruit lists to process. Check your logic.");
            return;
        }

        List<Fruit> currentList = fruitLists[currentListIndex].fruitList;
        for (int i = 0; i < currentList.Count; i++)
        {
            if (!currentList[i].IsCollected)
            {
                return;
            }
        }

        Uicontroller.UpdateProgress();
        currentListIndex++;

        if (currentListIndex < fruitLists.Count)
        {
            ActivateNextList();
        }
        else
        {
            Debug.Log("Victory Achieved! All fruits collected.");

            OnVictoryAchieved?.Invoke();

            if (playerMovement != null && playerMovement.playerCollider != null)
            {
                playerMovement.playerCollider.enabled = false;
            }
            
        }
    }

    private void ActivateNextList()
    {
        if (currentListIndex >= fruitLists.Count)
        {
            Debug.LogWarning("Attempted to activate a fruit list that doesn't exist.");
            return;
        }

        List<Fruit> nextList = fruitLists[currentListIndex].fruitList;
        for (int i = 0; i < nextList.Count; i++)
        {
            if (nextList[i] != null)
            {
                nextList[i].gameObject.SetActive(true);
            }
        }

        Uicontroller.MoveArrowToCurrentIndicator();

        Debug.Log($"Next fruit list activated. Current list index: {currentListIndex}");
    }
}
