using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "ScriptableObjects/Card")]
public class CardStats : ScriptableObject
{
    public string cardName;
    public string target;
    public int actionValue;
    public Color actionColor;
    public int range;
    public Sprite artwork;

}
