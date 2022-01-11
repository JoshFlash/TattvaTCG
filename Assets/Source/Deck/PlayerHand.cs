using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerHand : IEnumerable<Card>
{
    private readonly List<Card> cards = new();
    public bool IsEmpty => cards.Count == 0;
    public int Size => cards.Count;

    public List<Card> GetMovingCards()
    {
        return cards.FindAll(card => card.ShouldMove());
    }

    public void Add(Card card)
    {
        cards.Add(card);
    }
    
    public void Remove(Card card)
    {
        cards.Remove(card);
    }

    public void Clear()
    {
        cards.Clear();
    }

    public IEnumerator<Card> GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Card this[int index] => cards[index];

    public bool Contains(Card card)
    {
        return cards.Contains(card);
    }

    public Card GetLeftCard(Card referenceCard)
    {
        var index = cards.IndexOf(referenceCard);
        return index > 0 ? cards[index - 1] : null;
    }

    public Card GetRightCard(Card referenceCard)
    {
        var index = cards.IndexOf(referenceCard);
        return index < Size - 1 ? cards[index + 1] : null;
    }

    public Card GetCenterCard()
    {
        if (Size > 0)
        {
            return cards[Size / 2];
        }

        return null;
    }
}