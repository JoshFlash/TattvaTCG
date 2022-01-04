using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private static LayerMask kUiLayer = default;
    
    private const int kMaxHandSize = 10;
    private const float kCardDepthInterval = 0.01f;
    
    [SerializeField] private string cardPrefabLocation = "Cards/Card_01";
    
    private PlayerHand playerHand = new PlayerHand();
    
    private Card selectedCard = default;
    private Card examinedCard = default;
    private Card mouseOverCard = default;
    
    private RaycastHit[] results = new RaycastHit[30];
    
    public Camera Camera { get; private set; }
    
    private void Awake()
    {
        kUiLayer = LayerMask.GetMask("UI");
        Camera ??= Camera.main;
    }

    private void Update()
    {
        if (playerHand.IsEmpty) return;

        CheckMouseOverCard(out mouseOverCard);
        
        if (Input.GetMouseButtonUp(0) && TrySelectCard())
        {
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            // play selected card
        }

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
        if (examinedCard != null)
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
            card.MoveToRequestedPosition(CardConfig.Instance.MoveSpeed);
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
                var examinedDistance = GetExaminedCardSqrDistance(result.point);
                var distance = Vector3.SqrMagnitude(result.point - hitCard.transform.position);
                if (distance < minSqrDistance && distance < examinedDistance)
                {
                    minSqrDistance = distance;
                    mouseOver = hitCard;
                }
            }
        }
    }

    private float GetExaminedCardSqrDistance(Vector3 point)
    {
        return examinedCard != null ? Vector3.SqrMagnitude(point - examinedCard.defaultPosition) : float.MaxValue;
    }

    private bool TrySelectCard()
    {
        if (playerHand.Contains(mouseOverCard))
        {
            selectedCard?.SetState(CardState.Default);
            if (selectedCard != mouseOverCard)
            {
                selectedCard = mouseOverCard;
                selectedCard.SetState(CardState.Select);
            }
            else
            {
                selectedCard = null;
            }

            return true;
        }

        return false;
    }

    public async UniTask<Card> AddCard()
    {
        Card cardInstance = null;
        if (playerHand.Size < kMaxHandSize)
        {
            var cardPrefab = Resources.Load<Card>(cardPrefabLocation);
            cardInstance = Instantiate(cardPrefab, transform.position, transform.rotation, transform);
            playerHand.Add(cardInstance);
            cardInstance.TweenToPosition(cardInstance.transform.position + new Vector3(0, CardConfig.Instance.SelectHeight, 0), CardConfig.Instance.DealtSpeed);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.Instance.DealtSpeed));
            
            AdjustPositions(transform.position);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.Instance.SortSpeed));
        }

        return cardInstance;
    }

    public async UniTask DiscardCard(Card card)
    {
        card.TweenToPosition(CardDefaultPosition(kMaxHandSize, (int)(kMaxHandSize * 1.5f)), CardConfig.Instance.DealtSpeed);
        card.LockPosition = true;
        playerHand.Remove(card);

        await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.Instance.DealtSpeed));
        Destroy(card.gameObject);
    }

    private void AdjustPositions(Vector3 handAnchorPosition)
    {
        int index = -1;
        foreach (var card in playerHand)
        {
            int offset = playerHand.Size / 2;
            card.TweenToPosition(handAnchorPosition + CardDefaultPosition(offset, ++index), CardConfig.Instance.SortSpeed, card.CachePosition);
        }
    }
    
    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            playerHand.Size < kMaxHandSize / 2 ? CardConfig.Instance.MaxPadding :
            playerHand.Size >= (int)(kMaxHandSize * 0.8f) ? CardConfig.Instance.MinPadding :
            (CardConfig.Instance.MaxPadding + CardConfig.Instance.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, 0, kCardDepthInterval * index);
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
        if (examinedCard != null)
        {
            ClearAdjacentCards(examinedCard, selectedCard);

            examinedCard.SetState(CardState.Default);
            examinedCard = null;
        }
    }

    private void ClearSelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.SetState(CardState.Default);
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
        foreach (var cardData in playerHand)
        {
            cardData.LockPosition = false;
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

    private void ClearAdjacentCards(Card examinedCard, Card selectedCard)
    {
        var leftCard = playerHand.GetLeftCard(examinedCard);
        var rightCard = playerHand.GetRightCard(examinedCard);
        
        if (leftCard && leftCard != selectedCard)
        {
            leftCard.SetState(CardState.Default);
        }

        if (rightCard && rightCard != selectedCard)
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

}