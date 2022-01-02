using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private const int kMaxHandSize = 10;
    private List<Card> cards = new();
    public bool IsEmpty => cards.Count == 0;
    private const float kCardDepthInterval = 0.01f;

    [SerializeField] private string cardPrefabLocation = "Cards/Card_01";
    
    private void LateUpdate()
    {
        foreach (Card card in cards)
        {
            if (card.ShouldMove())
            {
                card.TweenToRequestedPosition(CardConfig.GlobalSettings.MoveSpeed);
            }
        }
    }

    public async UniTask<Card> AddCard()
    {
        Card cardInstance = null;
        if (cards.Count < kMaxHandSize)
        {
            var cardPrefab = Resources.Load<Card>(cardPrefabLocation);
            cardInstance = Instantiate(cardPrefab, transform.position, transform.rotation, transform);
            cards.Add(cardInstance);
            cardInstance.TweenToPosition(transform.position + new Vector3(0, CardConfig.GlobalSettings.SelectHeight, 0), CardConfig.GlobalSettings.DealtSpeed);
            
            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                card.Index = i;
                CardConfig.GlobalSettings.ExamineDodgeDistance = (cards.Count - 1f) / kMaxHandSize * CardConfig.GlobalSettings.DodgeDistance;
                CardConfig.GlobalSettings.DodgeRightDistance = 1.25f * (cards.Count - 1f) / kMaxHandSize * CardConfig.GlobalSettings.DodgeDistance;
                int offset = cards.Count / 2;
                card.TweenToPosition(transform.position + CardDefaultPosition(offset, i), CardConfig.GlobalSettings.SortSpeed, card.CachePosition);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.SortSpeed));
        }

        return cardInstance;
    }

    public async UniTask DiscardCard(Card card)
    {
        card.TweenToPosition(CardDefaultPosition(kMaxHandSize, (int)(kMaxHandSize * 1.5f)), CardConfig.GlobalSettings.DealtSpeed);
        card.LockPosition = true;

        cards.Remove(card);

        await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
        Destroy(card.gameObject);
    }
    
    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            cards.Count < kMaxHandSize / 2 ? CardConfig.GlobalSettings.MaxPadding :
            cards.Count >= (int)(kMaxHandSize * 0.8f) ? CardConfig.GlobalSettings.MinPadding :
            (CardConfig.GlobalSettings.MaxPadding + CardConfig.GlobalSettings.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, 0, kCardDepthInterval * index);
    }

    public bool Contains(Card card)
    {
        return cards.Contains(card);
    }

    public void UpdateAdjacentCards(Card examinedCard, Card selectedCard)
    {
        if (examinedCard.Index > 0)
        {
            var leftCard = cards[examinedCard.Index - 1];
            if (leftCard != selectedCard)
            {
                leftCard.SetState(CardState.DodgeLeft);
            }
        }

        if (examinedCard.Index < cards.Count - 1)
        {
            var rightCard = cards[examinedCard.Index + 1];
            if (rightCard != selectedCard)
            {
                rightCard.SetState(CardState.DodgeRight);
            }
        }
    }

    public void ClearAdjacentCards(Card examinedCard, Card selectedCard)
    {
        if (examinedCard.Index > 0)
        {
            var leftCard = cards[examinedCard.Index - 1];
            if (leftCard != selectedCard)
            {
                leftCard.SetState(CardState.Default);
            }
        }

        if (examinedCard.Index < cards.Count - 1)
        {
            var rightCard = cards[examinedCard.Index + 1];
            if (rightCard != selectedCard)
            {
                rightCard.SetState(CardState.Default);
            }
        }
    }

    public void DestroyAllCards()
    {
        foreach (Card card in cards)
        {
            Destroy(card.gameObject);
        }

        cards.Clear();
    }

    public void UnlockAllCards()
    {
        cards.ForEach(card => card.LockPosition = false);
    }

}