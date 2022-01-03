using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CardState { Default, Examine, Select, DodgeLeft, DodgeRight, Clear }

public class HandController : MonoBehaviour
{
    private static LayerMask kUiLayer = default;
    
    private const int kMaxHandSize = 10;
    private const float kCardDepthInterval = 0.01f;
    
    private static Vector3 kExamineOffset      => new (0, CardConfig.GlobalSettings.ExamineHeight, CardConfig.GlobalSettings.ExamineDepth);
    private static Vector3 kClearOffset        => new (0, CardConfig.GlobalSettings.ExamineHeight / 2 , 0);
    private static Vector3 kSelectOffset       => new (0, CardConfig.GlobalSettings.SelectHeight, CardConfig.GlobalSettings.SelectDepth);
    private static Vector3 kDodgeOffsetLeft    => new (-CardConfig.GlobalSettings.DodgeDistance, 0, 0);
    private static Vector3 kDodgeOffsetRight   => new (CardConfig.GlobalSettings.DodgeDistance, 0, 0);

    [SerializeField] private string cardPrefabLocation = "Cards/Card_01";
    
    private HandData hand = new HandData();
    
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
        if (hand.IsEmpty) return;

        CheckMouseOverCard(out mouseOverCard);
        
        if (Input.GetMouseButtonUp(0) && TrySelectCard())
        {
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            // play selected card
        }

        if (mouseOverCard is null)
        {
            ClearExaminedCard();
        }
        else
        {
            UpdateExaminedCard();
        }
    }

    private void LateUpdate()
    {
        foreach (var card in hand.GetMovingCards(ShouldMove))
        {
            card.MoveToRequestedPosition(CardConfig.GlobalSettings.MoveSpeed);
        }
    }

    private bool ShouldMove(Card cardData) =>
        !cardData.LockPosition && cardData.TargetPositionCached != cardData.TargetPositionRequested;

    private Vector3 GetOffset(CardState state)
    {
        switch (state)
        {
            case CardState.Default:
                return Vector3.zero;
            case CardState.Examine:
                return kExamineOffset;
            case CardState.Clear:
                return kClearOffset;
            case CardState.Select:
                return kSelectOffset;
            case CardState.DodgeLeft:
                return kDodgeOffsetLeft;
            case CardState.DodgeRight:
                return kDodgeOffsetRight;
        }

        return Vector3.zero;
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
                var distance = Vector3.SqrMagnitude(result.point - hitCard.transform.position);
                if (mouseOver == null || distance < minSqrDistance)
                {
                    minSqrDistance = distance;
                    mouseOver = hitCard;
                }
            }
        }
    }

    private bool TrySelectCard()
    {
        if (hand.Contains(mouseOverCard))
        {
            selectedCard?.SetState(CardState.Default, GetOffset(CardState.Default));
            if (selectedCard != mouseOverCard)
            {
                selectedCard = mouseOverCard;
                selectedCard.SetState(CardState.Select, GetOffset(CardState.Select));
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
        if (hand.Size < kMaxHandSize)
        {
            var cardPrefab = Resources.Load<Card>(cardPrefabLocation);
            cardInstance = Instantiate(cardPrefab, transform.position, transform.rotation, transform);
            hand.Add(cardInstance);
            cardInstance.TweenToPosition(cardInstance.transform.position + new Vector3(0, CardConfig.GlobalSettings.SelectHeight, 0), CardConfig.GlobalSettings.DealtSpeed);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
            
            AdjustPositions(transform.position);

            await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.SortSpeed));
        }

        return cardInstance;
    }

    public async UniTask DiscardCard(Card card)
    {
        card.TweenToPosition(CardDefaultPosition(kMaxHandSize, (int)(kMaxHandSize * 1.5f)), CardConfig.GlobalSettings.DealtSpeed);
        card.LockPosition = true;
        hand.Remove(card);

        await UniTask.Delay(TimeSpan.FromSeconds(CardConfig.GlobalSettings.DealtSpeed));
        Destroy(card.gameObject);
    }

    private void AdjustPositions(Vector3 handAnchorPosition)
    {
        int index = -1;
        foreach (var card in hand)
        {
            int offset = hand.Size / 2;
            card.TweenToPosition(handAnchorPosition + CardDefaultPosition(offset, ++index), CardConfig.GlobalSettings.SortSpeed, card.CachePosition);
        }
    }
    
    private Vector3 CardDefaultPosition(int offset, int index)
    {
        float padding =
            hand.Size < kMaxHandSize / 2 ? CardConfig.GlobalSettings.MaxPadding :
            hand.Size >= (int)(kMaxHandSize * 0.8f) ? CardConfig.GlobalSettings.MinPadding :
            (CardConfig.GlobalSettings.MaxPadding + CardConfig.GlobalSettings.MinPadding) / 2f;
        return new((-offset + index + 0.5f) * padding, 0, kCardDepthInterval * index);
    }

    private void UpdateExaminedCard()
    {
        if (mouseOverCard != selectedCard && mouseOverCard != examinedCard)
        {
            ClearExaminedCard();
            examinedCard = mouseOverCard;
            examinedCard.SetState(CardState.Examine, GetOffset(CardState.Examine));

            UpdateAdjacentCards(examinedCard, selectedCard);
        }
    }

    private void ClearExaminedCard()
    {
        if (examinedCard != null)
        {
            ClearAdjacentCards(examinedCard, selectedCard);

            examinedCard.SetState(CardState.Clear, GetOffset(CardState.Clear));
            examinedCard = null;
        }
    }

    private void ClearSelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.SetState(CardState.Default, GetOffset(CardState.Default));
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
        foreach (var cardData in hand)
        {
            cardData.LockPosition = false;
        }
    }

    private void UpdateAdjacentCards(Card examinedCard, Card selectedCard)
    {
        var leftCard = hand.GetLeftCard(examinedCard);
        var rightCard = hand.GetRightCard(examinedCard);
        
        if (leftCard && leftCard != selectedCard)
        {
            leftCard.SetState(CardState.DodgeLeft, GetOffset(CardState.DodgeLeft));
        }

        if (rightCard && rightCard != selectedCard)
        {
            rightCard.SetState(CardState.DodgeRight, GetOffset(CardState.DodgeRight));
        }
    }

    private void ClearAdjacentCards(Card examinedCard, Card selectedCard)
    {
        var leftCard = hand.GetLeftCard(examinedCard);
        var rightCard = hand.GetRightCard(examinedCard);
        
        if (leftCard && leftCard != selectedCard)
        {
            leftCard.SetState(CardState.Default, GetOffset(CardState.Default));
        }

        if (rightCard && rightCard != selectedCard)
        {
            rightCard.SetState(CardState.Default, GetOffset(CardState.Default));
        }
    }

    private void DestroyAllCards()
    {
        foreach (Card card in hand)
        {
            Destroy(card.gameObject);
        }

        hand.Clear();
    }

}