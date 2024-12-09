using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private int score;

    [Header("Progress Indicators")]
    public List<Image> progressIndicators;
    public Color completedColor = Color.black;
    public Color defaultColor = Color.white;

    [Header("Arrow Indicator")]
    public GameObject arrowIndicator;
    public Vector2 arrowOffset = new Vector2(0, 30f);

    private int currentIndicatorIndex = 0;

    private void Start()
    {
        ResetProgressUI();
    }

    private void OnEnable()
    {
        Fruit.OnFruitCollected += HandleFruitCollected;
    }

    private void OnDisable()
    {
        Fruit.OnFruitCollected -= HandleFruitCollected;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        else
        {
            Debug.LogWarning("Score Text UI is not assigned in UiManager.");
        }
    }

    private void HandleFruitCollected(Fruit fruit)
    {
        AddScore(fruit.points);
    }

    public void UpdateProgress()
    {
        if (progressIndicators == null || progressIndicators.Count == 0)
        {
            Debug.LogWarning("Progress Indicators not assigned in UiManager.");
            return;
        }

        if (currentIndicatorIndex < progressIndicators.Count)
        {
            progressIndicators[currentIndicatorIndex].color = completedColor;

            currentIndicatorIndex++;

            if (currentIndicatorIndex < progressIndicators.Count)
            {
                MoveArrowToCurrentIndicator();
            }
            else
            {
                arrowIndicator.SetActive(false);
            }
        }
        else
        {
            Debug.Log("All fruit lists have been completed.");
        }
    }

    public void MoveArrowToCurrentIndicator()
    {
        if (arrowIndicator == null || progressIndicators == null || progressIndicators.Count == 0)
        {
            Debug.LogWarning("Arrow Indicator or Progress Indicators not assigned.");
            return;
        }

        if (currentIndicatorIndex < progressIndicators.Count)
        {
            RectTransform targetIcon = progressIndicators[currentIndicatorIndex].GetComponent<RectTransform>();
            RectTransform arrowRect = arrowIndicator.GetComponent<RectTransform>();

            if (targetIcon != null && arrowRect != null)
            {
                arrowIndicator.SetActive(true);
                arrowRect.position = targetIcon.position + (Vector3)arrowOffset;
            }
        }
    }

    public void ResetProgressUI()
    {
        for (int i = 0; i < progressIndicators.Count; i++)
        {
            progressIndicators[i].color = defaultColor;
        }

        currentIndicatorIndex = 0;

        StartCoroutine(InitializeArrowPosition());
    }

    private IEnumerator InitializeArrowPosition()
    {
        yield return null;

        MoveArrowToCurrentIndicator();
    }
}
