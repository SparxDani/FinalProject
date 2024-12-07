using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FruitList
{
    public List<Fruit> fruitList;
}

public class FruitManager : MonoBehaviour
{
    [SerializeField] public List<FruitList> fruitLists = new List<FruitList>();
    private int currentListIndex = 0;

    public static event Action OnVictoryAchieved;

    void Start()
    {
        for (int i = 0; i < fruitLists.Count; i++)
        {
            List<Fruit> list = fruitLists[i].fruitList;
            bool shouldActivate = (i == 0);
            for (int j = 0; j < list.Count; j++)
            {
                list[j].gameObject.SetActive(shouldActivate);
            }
        }
    }

    private void OnEnable()
    {
        Fruit.OnFruitCollected += UpdateScore;
    }

    private void OnDisable()
    {
        Fruit.OnFruitCollected -= UpdateScore;
    }

    private void UpdateScore(int points)
    {
        UiManager.Instance.AddScore(points);
    }

    public void OnFruitCollected(Fruit fruit)
    {
        CheckCurrentListCompletion();
    }

    private void CheckCurrentListCompletion()
    {
        List<Fruit> currentList = fruitLists[currentListIndex].fruitList;
        for (int i = 0; i < currentList.Count; i++)
        {
            if (!currentList[i].IsCollected)
            {
                return;
            }
        }

        currentListIndex++;
        if (currentListIndex < fruitLists.Count)
        {
            ActivateNextList();
        }
        else
        {
            OnVictoryAchieved?.Invoke();
        }
    }

    private void ActivateNextList()
    {
        if (currentListIndex < fruitLists.Count)
        {
            List<Fruit> nextList = fruitLists[currentListIndex].fruitList;
            for (int i = 0; i < nextList.Count; i++)
            {
                nextList[i].gameObject.SetActive(true);
            }
            Debug.Log("Next fruit list activated.");
        }
    }
}
