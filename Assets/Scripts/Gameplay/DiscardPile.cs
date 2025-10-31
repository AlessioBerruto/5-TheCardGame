using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


[ExecuteAlways]
public class DiscardPile : MonoBehaviour
{
    private Stack<Card> discardedCards = new Stack<Card>();
    private Card topCard;   

    public void AddCard(Card card)
    {
        if (card == null) return;

        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.Flip(true);
        discardedCards.Push(card);

        UpdateTopCard();
    }

    private void UpdateTopCard()
    {
        foreach (Card c in discardedCards)
        {
            c.gameObject.SetActive(false);
        }

        if (discardedCards.Count > 0)
        {
            topCard = discardedCards.Peek();
            topCard.gameObject.SetActive(true);
            topCard.transform.SetAsLastSibling();
        }
        else
        {
            topCard = null;
        }
    }

    public Card GetTopCard()
    {
        return topCard;
    }

    public Card TakeTopCard()
    {
        if (discardedCards.Count == 0) return null;
        Card taken = discardedCards.Pop();
        UpdateTopCard();
        return taken;
    }

    public int Count
    {
        get { return discardedCards.Count; }
    }

    public void ClearPile()
    {
        foreach (Card c in discardedCards)
        {
            c.ResetCard();
        }
        discardedCards.Clear();
        topCard = null;
    }
}
