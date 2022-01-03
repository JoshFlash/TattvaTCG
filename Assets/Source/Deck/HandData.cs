using System;
using System.Collections;
using System.Collections.Generic;

public class HandData : IEnumerable<Card>
{
    private readonly List<Card> handData = new();
    public bool IsEmpty => handData.Count == 0;
    public int Size => handData.Count;

    public List<Card> GetMovingCards()
    {
        return handData.FindAll(card => card.ShouldMove());
    }

    public void Add(Card card)
    {
        handData.Add(card);
    }
    
    public void Remove(Card card)
    {
        handData.Remove(card);
    }

    public void Clear()
    {
        handData.Clear();
    }

    public IEnumerator<Card> GetEnumerator()
    {
        return handData.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Card this[int index] => handData[index];

    public bool Contains(Card card)
    {
        return handData.Contains(card);
    }

    public Card GetLeftCard(Card referenceCard)
    {
        var index = handData.IndexOf(referenceCard);
        return index > 0 ? handData[index - 1] : null;
    }

    public Card GetRightCard(Card referenceCard)
    {
        var index = handData.IndexOf(referenceCard);
        return index < Size - 1 ? handData[index + 1] : null;
    }
}