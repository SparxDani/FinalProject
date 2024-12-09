using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject PausePanel;

    [Header("AudioSources")]
    public AudioSource audioSource;
    public AudioSource backgroundMusicSource;

    [Header("MusicAssets")]
    public AudioClip WinSound;
    public AudioClip VictoryMusic;
    public AudioClip LoseSound;
    public AudioClip DefeatSound;

    [Header("RectTransform Object")]
    public RectTransform WinPanelRectTransform;
    public RectTransform LosePanelRectTransform;
    public RectTransform PausePanelRectTransform;

    [Header("Visual Effects")]
    public GameObject victoryEffect; 

    private bool isPaused = false;
    private bool isAnimating = false;

    private void OnEnable()
    {
        FruitManager.OnVictoryAchieved += HandleVictory;
        CharacterMovement.OnPlayerDefeated += HandleDefeat;
        CharacterMovement.OnPlayerVictory += HandleVictory;
    }

    private void OnDisable()
    {
        FruitManager.OnVictoryAchieved -= HandleVictory;
        CharacterMovement.OnPlayerDefeated -= HandleDefeat;
        CharacterMovement.OnPlayerVictory -= HandleVictory; 
    }

    private void HandleVictory()
    {
        Debug.Log("HandleVictory triggered in GameController.");
        PlaySound(WinSound, false);
        StartCoroutine(HandleVictorySequence());
    }


    private void HandleDefeat()
    {
        PlaySound(LoseSound, false);
        StartCoroutine(HandleDefeatSequence());
    }

    private IEnumerator HandleDefeatSequence()
    {
        yield return new WaitForSeconds(2f);

        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }

        PlaySound(DefeatSound, false);
        if (LosePanel != null)
        {
            LosePanel.gameObject.SetActive(true);
            LosePanelRectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutBounce);
        }
    }

    private IEnumerator HandleVictorySequence()
    {
        yield return new WaitForSeconds(2f);

        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }

        PlaySound(VictoryMusic, false);

        if (victoryEffect != null)
        {
            Instantiate(victoryEffect, Vector3.zero, Quaternion.identity);
        }

        if (WinPanel != null)
        {
            WinPanel.gameObject.SetActive(true);
            WinPanelRectTransform.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.OutBounce);
        }
    }

    private void PlaySound(AudioClip clip, bool loop = false)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void LoadScene(string nameScene)
    {
        Time.timeScale = 1f;
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(nameScene);
        }
        else
        {
            Debug.LogError("SceneTransitionManager is not available in the current scene.");
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneTransitionManager.Instance.TransitionToScene(currentSceneName);
    }

    public void PauseGame()
    {
        if (isPaused || isAnimating) return;
        isAnimating = true;

        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }

        PausePanel.SetActive(true);
        PausePanelRectTransform.anchoredPosition = new Vector2(0, 1000);
        PausePanelRectTransform
            .DOAnchorPos(Vector2.zero, 1f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                Time.timeScale = 0f;
                isPaused = true;
                isAnimating = false;
            });
    }

    public void ResumeGame()
    {
        if (!isPaused || isAnimating) return;
        isAnimating = true;

        Time.timeScale = 1f;

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.UnPause();
        }

        PausePanelRectTransform
            .DOAnchorPos(new Vector2(0, 1000), 1f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                PausePanel.SetActive(false);
                isPaused = false;
                isAnimating = false;
            });
    }
}