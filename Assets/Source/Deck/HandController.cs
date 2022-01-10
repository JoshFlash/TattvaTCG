using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private static LayerMask kUiLayer = default;
    private const int kMaxHandSize = 10;
    
    [SerializeField] private string cardPrefabLocation = "Cards/Card_01";
    
    private PlayerHand playerHand = new();
    
    private Card selectedCard = default;
    private Card examinedCard = default;
    private Card mouseOverCard = default;
    
    private readonly RaycastHit[] results = new RaycastHit[30];
    private bool abeyInput = false;
    
    public Camera Camera { get; private set; }
    
    private void Awake()
    {
        kUiLayer = LayerMask.GetMask("UI");
        Camera ??= Camera.main;
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
            var examinedCardPoint = Camera.WorldToScreenPoint(examinedCard.transform.position);
            return mousePos.y > examinedCardPoint.y;
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

    private void CheckMouseOverCard(out Card mouseOver)
    {
        mouseOver = null;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        var minSqrDistance = float.MaxValue;
        var index = Physics.SphereCastNonAlloc(ray, 0.01f, results, 100f, kUiLayer);
        for (--index; index >= 0; --index)
        {
            var result = results[index];
            if (result.collider.TryGetComponent(out Card hitCard))
            {
                if (hitCard.LockInteraction) continue;
                
                var examinedDistance = GetExaminedCardSqrDistance(result.point);
                var distance = Vector3.SqrMagnitude(result.point - hitCard.transform.position);
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
                selectedCard.SetState(CardState.Select);
            }
        }
    }

    public async UniTask<Card> AddCard()
    {
        abeyInput = true;
        Card cardInstance = null;
        if (playerHand.Size < kMaxHandSize)
        {
            var cardPrefab = Resources.Load<Card>(cardPrefabLocation);
            cardInstance = Instantiate(cardPrefab, transform.position, transform.rotation, transform);
            playerHand.Add(cardInstance);
            cardInstance.TweenToPosition(cardInstance.transform.position + new Vector3(0, CardConfig.SelectHeight, 0), CardConfig.DealtSpeed);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.DealtSpeed));
            
            AdjustPositions(transform.position);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.SortSpeed));
        }

        abeyInput = false;
        return cardInstance;
    }

    public async UniTask DiscardCard(Card card, Vector3 handAnchorPosition)
    {
        card.TweenToPosition(handAnchorPosition + CardDefaultPosition(kMaxHandSize, -1), CardConfig.DealtSpeed);
        card.Lock();
        playerHand.Remove(card);
        
        AdjustPositions(transform.position);

        await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.DealtSpeed));
        Destroy(card.gameObject);
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

    private void ClearSelectedCard()
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

    private void UpdateAdjacentCards(Card examinedCard, Card selectedCard)
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

    private void ClearAdjacentCards(Card centerCard, Card lockedCard)
    {
        var leftCard = playerHand.GetLeftCard(centerCard);
        var rightCard = playerHand.GetRightCard(centerCard);
        
        if (leftCard && leftCard != lockedCard)
        {
            leftCard.SetState(CardState.Default);
        }

        if (rightCard && rightCard != lockedCard)
        {
            rightCard.SetState(CardState.Default);
        }
    }

    private void DestroyAllCards()
    {
        foreach (Card card in playerHand)
        {
            Destroy(card.gameObject);
        }

        playerHand.Clear();
    }

    public async UniTask<int> TryPlaySelectedCard(int championMana, int championSpellPower)
    {
        abeyInput = true;
        int manaSpent = 0;
        if (selectedCard?.ManaCost <= championMana)
        {
            var target = await SelectTarget();

            if (selectedCard.PlayCard(target))
            {
                await DiscardCard(selectedCard, transform.position);
                manaSpent = selectedCard.ManaCost;
            }
        }

        abeyInput = false;
        return manaSpent;
    }

    private async UniTask<ICharacter> SelectTarget()
    {
        await UniTask.Yield();
        var winion = FindObjectOfType<Minion>();
        return winion;
    }
}