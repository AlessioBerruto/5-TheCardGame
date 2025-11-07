using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    private List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        if (card == null) return;

        cards.Add(card);
        card.transform.SetParent(transform, false);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = Vector3.one;
        card.Flip(true);
        card.gameObject.SetActive(true);
        card.transform.SetAsLastSibling(); 
    }

    public Card GetTopCard()
    {
        if (cards.Count == 0)
            return null;
        return cards[cards.Count - 1];
    }

    public List<Card> GetAllCards()
    {
        return new List<Card>(cards);
    }

    public void ClearPile()
    {
        foreach (Card card in cards)
        {
            if (card != null)
                card.gameObject.SetActive(false);
        }
        cards.Clear();
    }

    public int Count => cards.Count;
}
