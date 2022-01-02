using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleDeckController : MonoBehaviour
{
    private static LayerMask kUiLayer = default;
    
    [SerializeField] private HandController hand = default;

    private Card selectedCard = default;
    private Card examinedCard = default;
    private Card mouseOverCard = default;

    private RaycastHit[] results = new RaycastHit[30];

    private void Awake()
    {
        kUiLayer = LayerMask.GetMask("UI");
    }

    private void Update()
    {
        if (hand.IsEmpty) return;

        CheckMouseOverCard(out mouseOverCard);
        
        if (Input.GetMouseButtonUp(0) && TrySelectCard(mouseOverCard))
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
        else if (examinedCard != mouseOverCard)
        {
            UpdateExaminedCard();
        }
    }

    private void CheckMouseOverCard(out Card mouseOver)
    {
        mouseOver = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var index = Physics.RaycastNonAlloc(ray, results, 100f, kUiLayer);
        for (--index; index >= 0; --index)
        {
            var result = results[index];
            Log.Info(result.collider.name);
            if (result.collider.TryGetComponent(out Card hitCard))
            {
                if (mouseOver == null || IsPointCloser(result.point, hitCard.transform.position, mouseOver.transform.position))
                {
                    mouseOver = hitCard;
                }
            }
        }
        
        bool IsPointCloser(Vector3 origin, in Vector3 testPoint, in Vector3 controlPoint) =>
            Vector3.SqrMagnitude(origin - testPoint) < Vector3.SqrMagnitude(origin - controlPoint);
    }

    public async UniTask<Card> AddCard()
    {
        return await hand.AddCard();
    }

    private bool TrySelectCard(Card card)
    {
        if (hand.Contains(card))
        {
            if (selectedCard != card)
            {
                ClearSelectedCard();
                selectedCard = card;
                selectedCard?.SetState(CardState.Select);
            }
            else
            {
                ClearSelectedCard();
            }

            return true;
        }

        return false;
    }

    private void UpdateExaminedCard()
    {
        if (mouseOverCard != selectedCard && mouseOverCard != examinedCard)
        {
            ClearExaminedCard();
            examinedCard = mouseOverCard;
            examinedCard.SetState(CardState.Examine);

            hand.UpdateAdjacentCards(examinedCard, selectedCard);
        }
    }

    private void ClearExaminedCard()
    {
        if (examinedCard != null)
        {
            hand.ClearAdjacentCards(examinedCard, selectedCard);

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

    public void ClearHand()
    {
        ClearExaminedCard();
        ClearSelectedCard();
        hand.DestroyAllCards();
    }

    public void UnlockCards()
    {
        hand.UnlockAllCards();
    }
}