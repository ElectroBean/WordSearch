using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Value Word", menuName = "Value Words")]
public class ValueWord : ScriptableObject
{
    public string word;
    public string description;

    public Sprite wordArtwork;
}
