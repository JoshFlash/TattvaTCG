using System;
using System.Collections.Generic;

// a saved deck build that should be stored with as persistant save data
// accessible in many places, always sorted
public class SavedDeck
{
    // sorts by rarity then by name
    public List<PlayerCard> Cards { get; }
    private readonly IComparer<PlayerCard> playerCardRarityComparer;

    public SavedDeck() {}

    public SavedDeck (StarterDeck starterDeck)
    {
        playerCardRarityComparer = new PlayerCardRarityComparer();
        Cards = new (starterDeck.PlayerCards);
        
        Cards.Sort(playerCardRarityComparer);
    }

    public void Add(PlayerCard playerCard)
    {
        Cards.Add(playerCard);
        Cards.Sort(playerCardRarityComparer);
    }
    
}

public class PlayerCardRarityComparer : IComparer<PlayerCard>
{
    public int Compare(PlayerCard cardA, PlayerCard cardB)
    {
        if (cardA!.Rarity != cardB!.Rarity)
        {
            return cardA.Rarity.CompareTo(cardB.Rarity);
        }
        
        // sort by name when rarity is equal
        return string.Compare(cardA.name, cardB.name, StringComparison.Ordinal);
    }
}