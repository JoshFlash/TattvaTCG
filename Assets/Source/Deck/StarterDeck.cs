using System.Collections.Generic;
using UnityEngine;

// a curated collection of cards to use as the saved deck in a fresh play-through
[CreateAssetMenu(fileName = "New StarterDeck", menuName = "Configs/Starter Deck")]
public class StarterDeck : ScriptableObject
{
    public List<PlayerCard> PlayerCards = new();
}