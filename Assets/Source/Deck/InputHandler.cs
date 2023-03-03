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
    // highlighted character is for characters in play to be raised slightly from their location
    private Character highlightedCharacter = default;
    // mouseover card is to track which card the mouse is hovering over
    private PlayerCard mouseOverCard = default;

    private bool abeyInput = false;

    public bool IsReceivingInput()
    {
        return !abeyInput;
    }

    public void UpdateCardFocus()
    {
        mouseOverCard = null;
        
        if (!playerHand.IsEmpty && CheckMouseOverCard(out mouseOverCard, handLayer))
        {
            UpdateExaminedCard();
        }
        if (mouseOverCard is null && CheckMouseOverCard(out mouseOverCard, characterLayer))
        {
            UpdateHighlightedCharacter();
        }
        
        if (mouseOverCard is null && ShouldClearExaminedCard())
        {
            ClearExaminedCard();
        }
    }

    private bool ShouldClearExaminedCard()
    {
        if (examinedCard is null || examinedCard == selectedCard)
            return false;
        
        if (highlightedCharacter is null || examinedCard == highlightedCharacter.Card)
        {
            return 
                Input.mousePosition.y > examinedCard.transform.position.WorldToScreenPoint().y
                || Vector2.Distance(Input.mousePosition, examinedCard.transform.position.WorldToScreenPoint()) > kClearDistance;
        }

        return false;
    }

    private bool CheckMouseOverCard(out PlayerCard mouseOver, int inputLayer)
    {
        mouseOver = null;
        var minDistance = float.MaxValue;

        var results = MainCamera.ScreenCast(inputLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out PlayerCard hitCard))
            {
                if (hitCard.BlockMouseover) continue;
                if (hitCard == examinedCard)
                {
                    mouseOver = hitCard;
                    break;
                }

                var distance = Vector3.Distance(result.point, hitCard.GetStablePosition());
                if (distance < minDistance)
                {
                    minDistance = distance;
                    mouseOver = hitCard;
                }
            }
        }

        return mouseOver is not null;
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
            examinedCard.GetComponent<Character>()?.OnHighlight(CardState.Examine);
            examinedCard.SetState(CardState.Examine);

            UpdateAdjacentCards(examinedCard, selectedCard);
        }
    }

    private void UpdateHighlightedCharacter()
    {
        if (mouseOverCard != examinedCard 
            && mouseOverCard.TryGetComponent(out Character mouseOverUnit))
        {
            ClearExaminedCard();
            examinedCard = mouseOverCard;
            
            highlightedCharacter = mouseOverUnit;
            highlightedCharacter.OnHighlight(CardState.Highlight);
        }
    }

    private void ClearExaminedCard()
    {
        if (highlightedCharacter != null)
        {
            highlightedCharacter.OnClearHighlight(CardState.ClearHighlight);
            highlightedCharacter = null;
            examinedCard = null;
        }
        
        if (examinedCard != null && examinedCard != selectedCard)
        {
            ClearAdjacentCards(examinedCard, selectedCard);
            
            examinedCard.GetComponent<Character>()?.OnClearHighlight(CardState.ClearFocus);
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
        Log.NotImplemented(" This may no longer be necessary to implement");
    }

    public async UniTask AssignActions()
    {
        var results = MainCamera.ScreenCast(characterLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out ActionButton actionButton))
            {
                await actionButton.character.AssignAction(actionButton.CombatAction);
            }
        }
    }
}