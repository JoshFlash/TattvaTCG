using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StarterDeck", menuName = "Configs/Starter Deck")]
public class StarterDeck : ScriptableObject
{
    public List<PlayerCard> PlayerCards = new();
}