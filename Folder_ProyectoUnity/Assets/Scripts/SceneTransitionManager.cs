using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public RectTransform transitionImage;
    public Vector2 initialPosition;
    public Vector2 finalPosition;
    public float transitionDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName)
    {
        transitionImage.DOAnchorPos(Vector2.zero, transitionDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);

            transitionImage.DOAnchorPos(finalPosition, transitionDuration).SetEase(Ease.InOutQuad);
        });
    }

    private void Start()
    {
        transitionImage.anchoredPosition = initialPosition;
    }
}