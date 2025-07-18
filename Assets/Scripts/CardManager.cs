using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : Singleton<CardManager>
{
    public List<Card> cards;
    public int curCard;
    public int useCardCount;

    protected override void Awake()
    {
        cards.AddRange(GetComponentsInChildren<Card>());
        useCardCount = 0;
    }

    public void NextCard()
    {
        useCardCount++;
        curCard++;
        curCard %= 3;
        if (useCardCount >= 54)
            TotalPoint.Instance.GameOver();
    }
}
