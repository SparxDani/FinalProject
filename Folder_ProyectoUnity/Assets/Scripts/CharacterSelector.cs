using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        selectedIndex = (selectedIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        selectedIndex = (selectedIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
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