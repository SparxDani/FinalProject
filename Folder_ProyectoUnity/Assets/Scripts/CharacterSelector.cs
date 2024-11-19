using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterSelector : MonoBehaviour
{
    public CharacterSO[] characters;
    public Image characterImageDisplay;
    public TMP_Text characterNameDisplay;

    private int selectedIndex = 0;

    void Start()
    {
        UpdateCharacterDisplay();
    }

    public void NextCharacter()
    {
        RectTransform rectTransform = characterImageDisplay.rectTransform;
        rectTransform.DOAnchorPosX(500, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            selectedIndex = (selectedIndex + 1) % characters.Length;
            UpdateCharacterDisplay();
            rectTransform.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBounce);
        });

        characterNameDisplay.rectTransform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo);
    }

    public void PreviousCharacter()
    {
        RectTransform rectTransform = characterImageDisplay.rectTransform;
        rectTransform.DOAnchorPosX(-500, 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
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
    }

    public void SelectCharacter()
    {
        Instantiate(characters[selectedIndex].characterPrefab, Vector3.zero, Quaternion.identity);
    }
}
