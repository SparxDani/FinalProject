using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelectorMenuTransitionManager : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform pressStartPanel;
    public RectTransform mainMenuPanel;
    public RectTransform characterSelectionPanel;
    public RectTransform optionsPanel;
    public RectTransform levelSelectorPanel;

    [Header("Character Selector")]
    public CharacterSO[] characters;
    public Image characterImageDisplay;
    public TMP_Text characterNameDisplay;

    [Header("Character Model")]
    public Transform characterContainer; // Contenedor donde se instanciarán los personajes
    private GameObject currentCharacterModel; // Referencia al personaje actual

    private int selectedIndex = 0;

    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public Vector2 offScreenOffset = new Vector2(0, 500);

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip buttonStartSound;

    [Header("Events")]
    public Action OnPressStart;
    public Action OnPlayClicked;
    public Action OnOptionsClicked;
    public Action OnCharacterSelected;
    public Action OnBackToMainMenu;
    public Action OnBackToCharacter;

    void Start()
    {
        pressStartPanel.gameObject.SetActive(true);
        mainMenuPanel.gameObject.SetActive(false);
        characterSelectionPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
        levelSelectorPanel.gameObject.SetActive(false);

        UpdateCharacterDisplay();
    }

    public void OnPressStartClicked()
    {
        PlaySound(buttonStartSound);

        pressStartPanel.DOAnchorPos(pressStartPanel.anchoredPosition + offScreenOffset, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => pressStartPanel.gameObject.SetActive(false));

        mainMenuPanel.gameObject.SetActive(true);
        mainMenuPanel.DOAnchorPos(Vector2.zero, animationDuration).SetEase(Ease.OutBack);

        OnPressStart?.Invoke();
    }

    public void NextCharacter()
    {
        PlaySound(buttonClickSound);

        RectTransform rectTransform = characterImageDisplay.rectTransform;
        float targetX = rectTransform.anchoredPosition.x + 500;

        rectTransform.DOAnchorPosX(targetX, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            selectedIndex = (selectedIndex + 1) % characters.Length;
            UpdateCharacterDisplay();
            rectTransform.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBounce);
        });

        characterNameDisplay.rectTransform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo);
    }

    public void PreviousCharacter()
    {
        PlaySound(buttonClickSound);

        RectTransform rectTransform = characterImageDisplay.rectTransform;
        float targetX = rectTransform.anchoredPosition.x - 500;

        rectTransform.DOAnchorPosX(targetX, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            selectedIndex = (selectedIndex - 1 + characters.Length) % characters.Length;
            UpdateCharacterDisplay();
            rectTransform.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBounce);
        });

        characterNameDisplay.rectTransform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo);
    }

    void UpdateCharacterDisplay()
    {
        characterNameDisplay.text = characters[selectedIndex].characterName;
        characterImageDisplay.sprite = characters[selectedIndex].characterImage;

        UpdateCharacterModel();
    }

    private void UpdateCharacterModel()
    {
        // Destruir el modelo anterior si existe
        if (currentCharacterModel != null)
        {
            Destroy(currentCharacterModel);
        }

        // Instanciar el nuevo modelo y asignarlo al contenedor
        if (characters[selectedIndex].characterPrefab != null)
        {
            currentCharacterModel = Instantiate(characters[selectedIndex].characterPrefab, characterContainer);
            currentCharacterModel.transform.localPosition = Vector3.zero;
            currentCharacterModel.transform.localRotation = Quaternion.identity;
        }
    }

    public void ShowPanel(RectTransform panelToShow, RectTransform[] panelsToHide)
    {
        for (int i = 0; i < panelsToHide.Length; i++)
        {
            RectTransform panel = panelsToHide[i];
            Vector2 targetPosition = panel.anchoredPosition + offScreenOffset;
            panel.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.InBack)
                 .OnComplete(() => panel.gameObject.SetActive(false));
        }

        panelToShow.gameObject.SetActive(true);
        Vector2 startPosition = panelToShow.anchoredPosition - offScreenOffset;
        panelToShow.anchoredPosition = startPosition;
        panelToShow.DOAnchorPos(Vector2.zero, animationDuration).SetEase(Ease.OutBack);
    }

    public void OnPlayButtonClicked()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { mainMenuPanel };
        ShowPanel(characterSelectionPanel, panelsToHide);

        OnPlayClicked?.Invoke();
    }

    public void OnCharacterSelectClicked()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { characterSelectionPanel };
        ShowPanel(levelSelectorPanel, panelsToHide);

        OnCharacterSelected?.Invoke();
    }

    public void OnOptionsButtonClicked()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { mainMenuPanel };
        ShowPanel(optionsPanel, panelsToHide);

        OnOptionsClicked?.Invoke();
    }

    public void OnBackToMainMenuFromCharacter()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { characterSelectionPanel };
        ShowPanel(mainMenuPanel, panelsToHide);

        OnBackToMainMenu?.Invoke();
    }

    public void OnBackToMainMenuFromOptions()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { optionsPanel };
        ShowPanel(mainMenuPanel, panelsToHide);

        OnBackToMainMenu?.Invoke();
    }

    public void OnBackToCharacterFromLevels()
    {
        PlaySound(buttonClickSound);

        RectTransform[] panelsToHide = { levelSelectorPanel };
        ShowPanel(characterSelectionPanel, panelsToHide);

        OnBackToCharacter?.Invoke();
    }

    public void OnExitButtonClicked()
    {
        PlaySound(buttonClickSound);
        Application.Quit();
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
