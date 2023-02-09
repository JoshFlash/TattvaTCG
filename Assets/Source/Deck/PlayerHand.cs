using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerHand : IEnumerable<PlayerCard>
{
    private readonly List<PlayerCard> cards = new();
    public readonly int kMaxHandSize = 10;

    public bool IsEmpty => cards.Count == 0;
    public bool IsFull => cards.Count >= kMaxHandSize;
    public int Size => cards.Count;

    public List<PlayerCard> GetMovingCards()
    {
        return cards.FindAll(card => card.ShouldMove());
    }

    public void Add(PlayerCard playerCard)
    {
        cards.Add(playerCard);
    }
    
    public void Remove(PlayerCard playerCard)
    {
        cards.Remove(playerCard);
    }

    public void Clear()
    {
        cards.Clear();
    }

    public IEnumerator<PlayerCard> GetEnumerator()
    {
        return cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public PlayerCard this[int index] => cards[index];

    public bool Contains(PlayerCard playerCard)
    {
        return cards.Contains(playerCard);
    }

    public PlayerCard GetLeftCard(PlayerCard referencePlayerCard)
    {
        var index = cards.IndexOf(referencePlayerCard);
        return index > 0 ? cards[index - 1] : null;
    }

    public PlayerCard GetRightCard(PlayerCard referencePlayerCard)
    {
        var index = cards.IndexOf(referencePlayerCard);
        return index < Size - 1 ? cards[index + 1] : null;
    }

    public PlayerCard GetCenterCard()
    {
        if (Size > 0)
        {
            return cards[Size / 2];
        }

        return null;
    }
}