using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject WinPanel;
    public GameObject LosePanel;
    public AudioSource audioSource;
    public AudioSource backgroundMusicSource;

    public AudioClip WinSound;
    public AudioClip VictoryMusic;
    public AudioClip LoseSound;
    public AudioClip DefeatSound;

    public RectTransform WinPanelRectTransform;
    public RectTransform LosePanelRectTransform;

    private void OnEnable()
    {
        FruitManager.OnVictoryAchieved += HandleVictory;
        CharacterMovement.OnPlayerDefeated += HandleDefeat;
    }

    private void OnDisable()
    {
        FruitManager.OnVictoryAchieved -= HandleVictory;
        CharacterMovement.OnPlayerDefeated -= HandleDefeat;
    }

    private void HandleVictory()
    {
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
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneTransitionManager.Instance.TransitionToScene(currentSceneName);
    }
}
