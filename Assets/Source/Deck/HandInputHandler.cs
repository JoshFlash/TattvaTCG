using System;
using Cysharp.Threading.Tasks;
using TweenKey;
using UnityEngine;

public class HandInputHandler
{
    public HandInputHandler(PlayerHand playerHand)
    {
        this.playerHand = playerHand;
        handLayer = LayerMask.GetMask("Cards");
    }
    
    private readonly int handLayer = default;
    private readonly PlayerHand playerHand;
    
    private PlayerCard selectedCard = default;
    private PlayerCard examinedCard = default;
    private PlayerCard mouseOverCard = default;
    
    private bool abeyInput = false;
    
    public bool IsReceivingInput => !(playerHand.IsEmpty || abeyInput);

    public void UpdateCardFocus()
    {
        CheckMouseOverCard(out mouseOverCard);
        if (mouseOverCard != null)
        {
            UpdateExaminedCard();
        }
        else if (ShouldClearExaminedCard())
        {
            ClearExaminedCard();
        }
    }

    private bool ShouldClearExaminedCard()
    {
        var mousePos = Input.mousePosition;
        if (examinedCard != null && examinedCard != selectedCard)
        {
            return mousePos.y > examinedCard.transform.position.WorldToScreenPoint().y;
        }

        return false;
    }

    private void CheckMouseOverCard(out PlayerCard mouseOver)
    {
        mouseOver = null;
        var minSqrDistance = float.MaxValue;

        var results = MainCamera.ScreenCast(handLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out PlayerCard hitCard))
            {
                if (hitCard.BlockMouseover) continue;
                
                var examinedDistance = GetExaminedCardSqrDistance(result.point);
                var distance = Vector3.SqrMagnitude(result.point - hitCard.GetStablePosition());
                if (distance < minSqrDistance && distance < examinedDistance - CardMovementConfig.SwapTolerance)
                {
                    minSqrDistance = distance;
                    mouseOver = hitCard;
                }
            }
        }
    }

    private float GetExaminedCardSqrDistance(Vector3 point)
    {
        return examinedCard != null ? Vector3.SqrMagnitude(point - examinedCard.DefaultPosition) : float.MaxValue;
    }

    public void UpdateSelectedCard()
    {
        if (playerHand.Contains(mouseOverCard))
        {
            var currentlySelectedCard = selectedCard;
            ClearSelectedCard();
            if (currentlySelectedCard != mouseOverCard)
            {
                selectedCard = mouseOverCard;
                selectedCard.SetState(CardState.Select, playerHand.GetCenterCard().DefaultPosition);
            }
        }
    }

    public async UniTask AddAndAdjust(PlayerCard card, Transform handAnchor)
    {
        abeyInput = true;
        card.transform.SetParent(handAnchor);
        card.transform.TweenByRotation(Quaternion.AngleAxis(180, -card.transform.up), 0.5f);

        var startPosition = CardDefaultPosition(0, 0) + handAnchor.position + card.transform.rotation * CardState.Select.Offset;
        card.TweenToPosition(startPosition, CardMovementConfig.DealtSpeed);
        
        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.DealtSpeed));
        
        AdjustPositions(handAnchor.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.SortSpeed));

        abeyInput = false;
    }

    public async UniTask DiscardCard(PlayerCard playerCard, Transform handAnchor)
    {
        playerCard.TweenToPosition(handAnchor.position + CardDefaultPosition(playerHand.kMaxHandSize, -1), CardMovementConfig.DealtSpeed);
        playerCard.Lock();
        playerHand.Remove(playerCard);
        
        AdjustPositions(handAnchor.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.DealtSpeed));
        GameObject.Destroy(playerCard.gameObject);
    }

    private void AdjustPositions(Vector3 handAnchorPosition)
    {
        int index = -1;
        foreach (var card in playerHand)
        {
            int offset = playerHand.Size / 2;
            card.TweenToPosition(handAnchorPosition + CardDefaultPosition(offset, ++index), CardMovementConfig.SortSpeed, card.CachePosition);
        }
    }
    
    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            playerHand.Size < playerHand.kMaxHandSize / 2 ? CardMovementConfig.MaxPadding :
            playerHand.Size >= (int)(playerHand.kMaxHandSize * 0.8f) ? CardMovementConfig.MinPadding :
            (CardMovementConfig.MaxPadding + CardMovementConfig.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, CardMovementConfig.DepthInterval * 2 * index, CardMovementConfig.DepthInterval * index);
    }

    private void UpdateExaminedCard()
    {
        if (mouseOverCard != selectedCard && mouseOverCard != examinedCard)
        {
            ClearExaminedCard();
            examinedCard = mouseOverCard;
            examinedCard.SetState(CardState.Examine);

            UpdateAdjacentCards(examinedCard, selectedCard);
        }
    }

    private void ClearExaminedCard()
    {
        if (examinedCard != null && examinedCard != selectedCard)
        {
            ClearAdjacentCards(examinedCard, selectedCard);

            examinedCard.SetState(CardState.ClearFocus);
            examinedCard = null;
        }
    }

    public void ClearSelectedCard()
    {
        if (selectedCard != null)
        {
            if (selectedCard == examinedCard) examinedCard = null;
            ClearAdjacentCards(selectedCard, null);
            
            selectedCard.SetState(CardState.ClearFocus);
            selectedCard = null;
        }
    }

    public void ClearAllCards()
    {
        ClearExaminedCard();
        ClearSelectedCard();
        DestroyAllCards();
    }

    public void UnlockAllCards()
    {
        foreach (var card in playerHand)
        {
            card.Unlock();
        }
    }

    private void UpdateAdjacentCards(PlayerCard examinedCard, PlayerCard selectedCard)
    {
        var leftCard = playerHand.GetLeftCard(examinedCard);
        var rightCard = playerHand.GetRightCard(examinedCard);
        
        if (leftCard && leftCard != selectedCard)
        {
            leftCard.SetState(CardState.DodgeLeft);
        }

        if (rightCard && rightCard != selectedCard)
        {
            rightCard.SetState(CardState.DodgeRight);
        }
    }

    private void ClearAdjacentCards(PlayerCard centerCard, PlayerCard lockedPlayerCard)
    {
        var leftCard = playerHand.GetLeftCard(centerCard);
        var rightCard = playerHand.GetRightCard(centerCard);
        
        if (leftCard && leftCard != lockedPlayerCard)
        {
            leftCard.SetState(CardState.Default);
        }

        if (rightCard && rightCard != lockedPlayerCard)
        {
            rightCard.SetState(CardState.Default);
        }
    }

    private void DestroyAllCards()
    {
        foreach (PlayerCard card in playerHand)
        {
            GameObject.Destroy(card.gameObject);
        }

        playerHand.Clear();
    }

    public async UniTask<int> PlaySelectedCard(ICharacter target, Transform handAnchor)
    {
        abeyInput = true;
        int manaSpent = 0;

        if (selectedCard.PlayCard(target))
        {
            await DiscardCard(selectedCard, handAnchor);
            manaSpent = selectedCard.ManaCost;
        }

        abeyInput = false;
        return manaSpent;
    }

    public bool TryPlaySelectedCard(int championMana)
    {
        if (selectedCard?.ManaCost <= championMana)
        {
            selectedCard.SetState(CardState.Target);
            return true;
        }
        
        return false;
    }
}