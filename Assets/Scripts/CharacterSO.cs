using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character Selector/Character")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public Sprite characterImage;
    public GameObject characterPrefab;
}

