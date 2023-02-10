using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

// the deck used during an encounter, loads from a selected saved deck
public class BattleDeck
{
    public BattleDeck(SavedDeck savedDeck)
    {
        deck = new List<PlayerCard>();
        foreach (var card in savedDeck.Cards)
        {
            PlayerCard cardInstance = GameObject.Instantiate(card);
            cardInstance.gameObject.SetActive(false);
            deck.Add(cardInstance);
        }
    }
    
    public PlayerHand PlayerHand { get; } = new();

    private List<PlayerCard> deck = new();
    private Queue<PlayerCard> drawPile = new();
    private Stack<PlayerCard> discardPile = new();
    private List<PlayerCard> banishedCards = new();

    private async UniTask<bool> AddCardToHand(HandInputHandler handInputHandler, Transform handAnchor)
    {
        if (PlayerHand.IsFull) return false;
        
        PlayerCard playerCard = drawPile.Dequeue();
        playerCard.gameObject.SetActive(true);
        
        PlayerHand.Add(playerCard);
        await handInputHandler.AddAndAdjust(playerCard, handAnchor);
        
        return true;
    }

    public async UniTask DrawCards(int drawCount, HandInputHandler handInputHandler, Transform handAnchor)
    {
        for (int i = 0; i < drawCount; i++)
        {
            bool didAdd = await AddCardToHand(handInputHandler, handAnchor);
            if (!didAdd)
            {
                Log.NotImplemented("TODO: implement logic for alerting the player when the hand is full");
            }
        }
    }

    public void ShuffleDeckIntoDrawPile()
    {
        List<PlayerCard> cardsInPlay = deck.Except(banishedCards).ToList();
        Probability.Shuffle(cardsInPlay);
        
        drawPile = new Queue<PlayerCard>(cardsInPlay);
    }
}