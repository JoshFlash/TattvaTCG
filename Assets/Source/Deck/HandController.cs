using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private static int kHandLayer = default;
    private const int kMaxHandSize = 10;
    
    [SerializeField] private string cardPrefabLocation = "Cards/Card_01";
    
    private PlayerHand playerHand = new();
    
    private PlayerCard selectedCard = default;
    private PlayerCard examinedCard = default;
    private PlayerCard mouseOverCard = default;
    
    private bool abeyInput = false;
    
    private void Awake()
    {
        kHandLayer = LayerMask.GetMask("Cards");
    }

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

    private void LateUpdate()
    {
        foreach (var card in playerHand.GetMovingCards())
        {
            card.MoveToRequestedPosition(CardConfig.MoveSpeed);
        }
    }

    private void CheckMouseOverCard(out PlayerCard mouseOver)
    {
        mouseOver = null;
        var minSqrDistance = float.MaxValue;

        var results = MainCamera.ScreenCast(kHandLayer);
        foreach (var result in results)
        {
            if (result.collider.TryGetComponent(out PlayerCard hitCard))
            {
                if (hitCard.BlockMouseover) continue;
                
                var examinedDistance = GetExaminedCardSqrDistance(result.point);
                var distance = Vector3.SqrMagnitude(result.point - hitCard.GetStablePosition());
                if (distance < minSqrDistance && distance < examinedDistance - CardConfig.SwapTolerance)
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

    public async UniTask<PlayerCard> AddCard()
    {
        abeyInput = true;
        PlayerCard playerCardInstance = null;
        if (playerHand.Size < kMaxHandSize)
        {
            var cardPrefab = GameServices.Get<DebugService>().BattleDebugData.PlayerDefaultCardInDeck.GetComponent<PlayerCard>();
            playerCardInstance = Instantiate(cardPrefab, transform.position, transform.rotation, transform);
            playerHand.Add(playerCardInstance);

            var startPosition = CardDefaultPosition(0, 0) + transform.position + playerCardInstance.transform.rotation * CardState.Select.Offset;
            playerCardInstance.TweenToPosition(startPosition, CardConfig.DealtSpeed);
            
            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.DealtSpeed));
            
            AdjustPositions(transform.position);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.SortSpeed));
        }

        abeyInput = false;
        return playerCardInstance;
    }

    public async UniTask DiscardCard(PlayerCard playerCard, Vector3 handAnchorPosition)
    {
        playerCard.TweenToPosition(handAnchorPosition + CardDefaultPosition(kMaxHandSize, -1), CardConfig.DealtSpeed);
        playerCard.Lock();
        playerHand.Remove(playerCard);
        
        AdjustPositions(transform.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.DealtSpeed));
        Destroy(playerCard.gameObject);
    }

    private void AdjustPositions(Vector3 handAnchorPosition)
    {
        int index = -1;
        foreach (var card in playerHand)
        {
            int offset = playerHand.Size / 2;
            card.TweenToPosition(handAnchorPosition + CardDefaultPosition(offset, ++index), CardConfig.SortSpeed, card.CachePosition);
        }
    }
    
    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            playerHand.Size < kMaxHandSize / 2 ? CardConfig.MaxPadding :
            playerHand.Size >= (int)(kMaxHandSize * 0.8f) ? CardConfig.MinPadding :
            (CardConfig.MaxPadding + CardConfig.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, CardConfig.DepthInterval * 2 * index, CardConfig.DepthInterval * index);
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
            Destroy(card.gameObject);
        }

        playerHand.Clear();
    }

    public async UniTask<int> PlaySelectedCard(ICharacter target)
    {
        abeyInput = true;
        int manaSpent = 0;

        if (selectedCard.PlayCard(target))
        {
            await DiscardCard(selectedCard, transform.position);
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