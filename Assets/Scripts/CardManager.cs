using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    public List<Card> cards;
    public int curCard;

    protected override void Awake()
    {
        cards.AddRange(GetComponentsInChildren<Card>());
    }

    public void NextCard()
    {
        curCard++;
        curCard %= 3;
    }
}
