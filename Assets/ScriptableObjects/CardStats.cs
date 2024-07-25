using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardStats : ScriptableObject
{
    public string cardName;
    public string description;
    public string actionType;
    public int actionValue;
    public Sprite artwork;

}
