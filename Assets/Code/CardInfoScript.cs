using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoScript : MonoBehaviour
{
    public CardStats card;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI target;
    private TextMeshProUGUI range;
    private TextMeshProUGUI actionValue;
    private Image cardArt;

    // Start is called before the first frame update
    void Awake()
    {
        cardArt = transform.Find("CardArt").GetComponent<Image>();
        nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        target = transform.Find("Target").GetComponent<TextMeshProUGUI>();
        range = transform.Find("Range").GetComponent <TextMeshProUGUI>();
        actionValue = transform.Find("ActionValue").GetComponent<TextMeshProUGUI>();

        cardArt.sprite = card.artwork;
        nameText.SetText(card.cardName);
        target.SetText(card.target);
        range.SetText(card.range.ToString());
        actionValue.SetText(card.actionValue.ToString());
        actionValue.color = card.actionColor;
    }

}
