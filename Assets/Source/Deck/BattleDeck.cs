using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            cardInstance.Deactivate();
            deck.Add(cardInstance);
        }
    }
    
    public PlayerHand PlayerHand { get; } = new();

    private List<PlayerCard> deck = new();
    private Queue<PlayerCard> drawPile = new();
    private Stack<PlayerCard> discardPile = new();
    private List<PlayerCard> banishedCards = new();

    private async UniTask<bool> AddCardToHand(HandInputHandler handInputHandler, Transform handAnchor, Transform drawAnchor)
    {
        if (PlayerHand.IsFull) return false;

        if (drawPile.Count == 0)
        {
            Log.NotImplemented("TODO - alert player when their discard and draw piles are both empty");
            await ShuffleDiscardIntoDrawPile();
        }
        
        PlayerCard playerCard = drawPile.Dequeue();
        playerCard.Activate();
        
        PlayerHand.Add(playerCard);
        await handInputHandler.AddAndAdjust(playerCard, handAnchor, drawAnchor);
        
        return true;
    }

    public async UniTask DrawCards(int drawCount, HandInputHandler handInputHandler, Transform handAnchor, Transform drawAnchor)
    {
        for (int i = 0; i < drawCount; i++)
        {
            bool didAdd = await AddCardToHand(handInputHandler, handAnchor, drawAnchor);
            if (!didAdd)
            {
                Log.NotImplemented("TODO - implement logic for alerting the player when the hand is full");
            }
        }
        
        // await one additional 'dealt' as both a pacing and a logic update buffer
        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.DealtSpeed));
    }

    public async UniTask ShuffleDeckIntoDrawPile()
    {
        List<PlayerCard> cardsInPlay = deck.Except(banishedCards).ToList();
        Probability.Shuffle(cardsInPlay);
        
        drawPile = new Queue<PlayerCard>(cardsInPlay);

        await UniTask.Yield();
    }
    
    public async UniTask ShuffleDiscardIntoDrawPile()
    {
        var shuffledCards = new List<PlayerCard>(drawPile);
        while (discardPile.Count > 0)
        {
            shuffledCards.Add(discardPile.Pop());
        }
        Probability.Shuffle(shuffledCards);
        
        drawPile = new Queue<PlayerCard>(shuffledCards);

        await UniTask.Yield();
    }

    public async UniTask<int> PlayCardOnTarget(PlayerCard card, ICharacter target, HandInputHandler handInputHandler, Transform handAnchor)
    {
        int manaSpent = 0;

        if (card.PlayCard(target))
        {
            if (card.Type.Equals(CardType.Summon))
            {
                await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.SummonSpeed));
            }
            else 
            {
                await handInputHandler.DiscardCard(card, handAnchor);
                discardPile.Push(card);
            }
            
            manaSpent = card.ManaCost;
            PlayerHand.Remove(card);
        }

        return manaSpent;
    }

    public async UniTask DiscardHand(HandInputHandler handInputHandler)
    {
        foreach (var playerCard in PlayerHand)
        {
            discardPile.Push(playerCard);
        }

        handInputHandler.ClearAllCards();

        await UniTask.Yield();
    }
}