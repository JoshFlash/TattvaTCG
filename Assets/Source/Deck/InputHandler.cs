using System;
using Cysharp.Threading.Tasks;
using TweenKey;
using UnityEngine;

public class InputHandler
{
    public InputHandler(PlayerHand playerHand)
    {
        this.playerHand = playerHand;
        handLayer = LayerMask.GetMask("Cards");
        characterLayer = LayerMask.GetMask("Units");
    }

    private const float kClearDistance = 280f;

    private readonly int characterLayer = default;
    private readonly int handLayer = default;
    private readonly PlayerHand playerHand;
    
    // selected card is viewed up close near screen center with extra details shown
    private PlayerCard selectedCard = default;
    // examined card is raised slightly from the hand to get a better quick look
    private PlayerCard  examinedCard = default;
    // mouseover card is to track which card the mouse is highlighting and does not have a specific view
    private PlayerCard mouseOverCard = default;

    private bool abeyInput = false;
    private int inputLayer = -1;

    public bool IsReceivingInput(Phase phase)
    {
        bool receiveInput = !abeyInput;
        if (phase.Equals(Phase.Spell))
            receiveInput &= !playerHand.IsEmpty;
        
        return receiveInput;
    }

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

    public void UpdateUnitFocus()
    {
        CheckMouseOverCard(out mouseOverCard);
        if (mouseOverCard != null)
        {
            UpdateHighlightedCharacter();
        }
        else if (ShouldClearHighlightedCharacter())
        {
            ClearHighlightedCard();
        }
    }

    private bool ShouldClearExaminedCard()
    {
        if (examinedCard != null && examinedCard != selectedCard)
        {
            return 
                Input.mousePosition.y > examinedCard.transform.position.WorldToScreenPoint().y
                || Vector2.Distance(Input.mousePosition, examinedCard.transform.position.WorldToScreenPoint()) > kClearDistance;
        }

        return false;
    }

    private bool ShouldClearHighlightedCharacter()
    {
        if (examinedCard != null && examinedCard != selectedCard)
        {
            return 
                Vector2.Distance(Input.mousePosition, examinedCard.transform.position.WorldToScreenPoint()) > kClearDistance;
        }

        return false;
    }

    private void CheckMouseOverCard(out PlayerCard mouseOver)
    {
        mouseOver = null;
        var minSqrDistance = float.MaxValue;

        var results = MainCamera.ScreenCast(inputLayer);
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

    public async UniTask AddAndAdjust(PlayerCard card, Transform handAnchor, Transform drawAnchor)
    {
        abeyInput = true;
        card.transform.position = drawAnchor.position;
        card.transform.SetParent(handAnchor);

        var startPosition = CardDefaultPosition(0, 0) + handAnchor.position + card.transform.rotation * CardState.Select.Offset;
        card.TweenToPosition(startPosition, CardMovementConfig.DealtSpeed);
        card.transform.TweenByRotation(Quaternion.AngleAxis(180, -card.transform.up), 0.5f);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.DealtSpeed));
        
        AdjustPositions(handAnchor.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.SortSpeed));

        abeyInput = false;
    }

    public async UniTask DiscardCard(PlayerCard playerCard, Transform handAnchor)
    {
        abeyInput = true;

        playerCard.TweenToPosition(handAnchor.position + CardDefaultPosition(playerHand.kMaxHandSize, -1), CardMovementConfig.DealtSpeed);
        playerCard.Lock();
        playerHand.Remove(playerCard);
        
        AdjustPositions(handAnchor.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.DealtSpeed));
        playerCard.Deactivate();
        
        abeyInput = false;
    }
    
    public async UniTask RemoveSummonCard(PlayerCard playerCard, Transform handAnchor)
    {
        abeyInput = true;

        playerHand.Remove(playerCard);
        
        AdjustPositions(handAnchor.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardMovementConfig.SummonSpeed));
        
        abeyInput = false;
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
        return new((-offset + index + 0.5f) * padding, 0, CardMovementConfig.DepthInterval * index);
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

    private void UpdateHighlightedCharacter()
    {
        var mouseOverUnit = mouseOverCard.GetComponent<Character>(); 
        if (mouseOverUnit && mouseOverUnit.IsFriendly && mouseOverCard != examinedCard)
        {
            ClearHighlightedCard();
            examinedCard = mouseOverCard;
            examinedCard.SetState(CardState.Highlight);
            
            examinedCard.Unlock();
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

    private void ClearHighlightedCard()
    {
        if (examinedCard != null && examinedCard != selectedCard)
        {
            examinedCard.SetState(CardState.ClearHighlight);
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
        DiscardAllCards();
    }

    public void UnlockAllCardsInHand()
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

    private void DiscardAllCards()
    {
        foreach (PlayerCard playerCard in playerHand)
        {
            playerCard.Deactivate();
        }

        playerHand.Clear();
    }

    public bool TryPlayCard(int championMana, out PlayerCard playedCard)
    {
        playedCard = null;
        if (mouseOverCard is null)
        {
            if (selectedCard?.ManaCost <= championMana)
            {
                playedCard = selectedCard;
                selectedCard.SetState(CardState.Target);
                return true;
            }
        }
        else if (mouseOverCard.ManaCost <= championMana)
        {
            playedCard = mouseOverCard;
            mouseOverCard.SetState(CardState.Target);
            return true;
        }
        
        return false;
    }

    public void SetPhase(Phase phase)
    {
        if (phase.Equals(Phase.Spell))
        {
            inputLayer = handLayer;
        }
        else if (phase.Equals(Phase.Ability))
        {
            inputLayer = characterLayer;
        }
        else
        {
            inputLayer = -1;
        }
    }

    public async UniTask<bool> TryAssignActions()
    {
        var results = MainCamera.ScreenCast(inputLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out ActionButton actionButton))
            {
                await actionButton.character.AssignAction(actionButton.CombatAction);
                return true;
            }
        }

        return false;
    }
}