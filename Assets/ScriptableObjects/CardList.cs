using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new CardList", menuName = "ScriptableObjects/CardList")]
public class CardList : ScriptableObject
{
    public List<CardStats> cardsAvailable;
    public List<CardStats> cardsNotAvailable;




}
