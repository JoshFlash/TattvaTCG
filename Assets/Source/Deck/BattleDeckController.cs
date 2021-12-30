using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleDeckController : MonoBehaviour
{
    public string CardPrefabLocation = "Cards/Card_01";

    private const float CARD_DEPTH_INTERVAL = 0.0003f;
    private const int MAX_HAND_SIZE = 10;

    public Transform HandAnchor = default;

    public Card SelectedCard { get; private set; } = default;

    private Card examinedCard = default;

    private List<Card> cardsInHand = new();

    public Card CheckMouseoverCard()
    {
        if (cardsInHand.Count == 0) return null;

        Card mouseOverCard = null;
        float minDistance = CardConfig.GlobalSettings.MouseOverBounds.x;
        foreach (Card card in cardsInHand)
        {
            var sPos = Camera.main.WorldToScreenPoint(card.transform.position);
            var xDist = MathF.Abs(sPos.x - Input.mousePosition.x);
            var yDist = MathF.Abs(sPos.y - Input.mousePosition.y);
            if (xDist <= minDistance && xDist < CardConfig.GlobalSettings.MouseOverBounds.x && 
                (yDist < CardConfig.GlobalSettings.MouseOverBounds.y || Input.mousePosition.y > 1 - CardConfig.GlobalSettings.CardAreaPadding.y))
            {
                minDistance = xDist;
                mouseOverCard = card;
            }
        }

        return mouseOverCard;
    }

    public async Task<Card> AddCard()
    {
        Card cardInstance = null;
        if (cardsInHand.Count < MAX_HAND_SIZE)
        {
            var cardPrefab = Resources.Load<Card>(CardPrefabLocation);
            cardInstance = Instantiate(cardPrefab, HandAnchor.position, Quaternion.identity, HandAnchor);
            cardsInHand.Add(cardInstance);
            cardInstance.MoveToPosition(new Vector3(0, CardConfig.GlobalSettings.SelectHeight, 0), CardConfig.GlobalSettings.DealtSpeed);
            await Task.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                var card = cardsInHand[i];
                card.Index = i;
                CardConfig.GlobalSettings.ExamineDodgeDistance = (cardsInHand.Count - 1f) / MAX_HAND_SIZE * CardConfig.GlobalSettings.DodgeDistance;
                CardConfig.GlobalSettings.DodgeRightDistance = 1.25f * (cardsInHand.Count - 1f) / MAX_HAND_SIZE * CardConfig.GlobalSettings.DodgeDistance;
                int offset = cardsInHand.Count / 2;
                card.MoveToPosition(CardDefaultPosition(offset, i), CardConfig.GlobalSettings.SortSpeed, card.CachePosition);
            }

            await Task.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.SortSpeed));
        }

        return cardInstance;
    }

    public async Task DiscardCard(Card card)
    {
        card.MoveToPosition(CardDefaultPosition(MAX_HAND_SIZE, (int)(MAX_HAND_SIZE * 1.5f)), CardConfig.GlobalSettings.DealtSpeed);
        card.LockPosition = true;

        if (card == SelectedCard) ClearSelectedCard();
        if (card == examinedCard) ClearExaminedCard();
        cardsInHand.Remove(card);

        await Task.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
        Destroy(card.gameObject);
    }

    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            cardsInHand.Count < MAX_HAND_SIZE / 2 ? CardConfig.GlobalSettings.MaxPadding :
            cardsInHand.Count >= (int)(MAX_HAND_SIZE * 0.8f) ? CardConfig.GlobalSettings.MinPadding :
            (CardConfig.GlobalSettings.MaxPadding + CardConfig.GlobalSettings.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, 0, CARD_DEPTH_INTERVAL * index);
    }

    public bool TrySelectCard(Card card)
    {
        if (cardsInHand.Contains(card))
        {
            if (examinedCard == card)
            {
                ClearExaminedCard();
            }

            if (SelectedCard != card)
            {
                ClearSelectedCard();
                SelectedCard = card;
                SelectedCard?.SetState(CardState.Select);
            }
            else
            {
                ClearSelectedCard();
            }

            return true;
        }

        return false;
    }

    public void UpdateExaminedCard(Card card)
    {
        if (cardsInHand.Contains(card) && card != SelectedCard && card != examinedCard)
        {
            ClearExaminedCard();
            examinedCard = card;
            examinedCard.SetState(CardState.Examine);

            if (examinedCard.Index > 0)
            {
                var leftCard = cardsInHand[examinedCard.Index - 1];
                if (leftCard != SelectedCard)
                {
                    leftCard.SetState(CardState.DodgeLeft);
                }
            }

            if (examinedCard.Index < cardsInHand.Count - 1)
            {
                var rightCard = cardsInHand[examinedCard.Index + 1];
                if (rightCard != SelectedCard)
                {
                    rightCard.SetState(CardState.DodgeRight);
                }
            }
        }
    }

    public void ClearExaminedCard()
    {
        if (examinedCard != null)
        {
            if (examinedCard.Index > 0)
            {
                var leftCard = cardsInHand[examinedCard.Index - 1];
                if (leftCard != SelectedCard)
                {
                    leftCard.SetState(CardState.Default);
                }
            }

            if (examinedCard.Index < cardsInHand.Count - 1)
            {
                var rightCard = cardsInHand[examinedCard.Index + 1];
                if (rightCard != SelectedCard)
                {
                    rightCard.SetState(CardState.Default);
                }
            }

            examinedCard.SetState(CardState.Default);
            examinedCard = null;
        }
    }

    public void ClearSelectedCard()
    {
        if (SelectedCard != null)
        {
            if (SelectedCard.Index > 0)
            {
                cardsInHand[SelectedCard.Index - 1].SetState(CardState.Default);
            }

            if (SelectedCard.Index < cardsInHand.Count - 1)
            {
                cardsInHand[SelectedCard.Index + 1].SetState(CardState.Default);
            }

            SelectedCard.SetState(CardState.Default);
            SelectedCard = null;
        }
    }

    public void Clear()
    {
        ClearExaminedCard();
        ClearSelectedCard();
        foreach (Card card in cardsInHand)
        {
            Destroy(card.gameObject);
        }

        cardsInHand.Clear();
    }

    public bool ShouldClearExaminedCard()
    {
        var cardAreaPadding = CardConfig.GlobalSettings.CardAreaPadding;
        return SelectedCard != examinedCard &&
               (Input.mousePosition.y < 1 - cardAreaPadding.y) ||
               (Input.mousePosition.x > 1 - cardAreaPadding.x) ||
               (Input.mousePosition.x < cardAreaPadding.x);
    }

    public void UnlockCards()
    {
        cardsInHand.ForEach(card => card.LockPosition = false);
    }
}