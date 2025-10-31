using UnityEngine;
using System.Collections.Generic;

public class CoveredPile : MonoBehaviour
{
    private Stack<Card> cards = new Stack<Card>();

    public void InitPile(List<Card> cardsList)
    {
        ClearPile();

        foreach (Card c in cardsList)
        {
            AddCard(c, false);
        }
    }

    public void AddCard(Card card, bool faceUp = false)
    {
        if (card == null) return;

        card.transform.SetParent(transform, false);
        card.transform.localPosition = new Vector3(0, cards.Count * 2f, 0);
        card.Flip(faceUp);
        card.gameObject.SetActive(true); 
        cards.Push(card);
    }

    public Card DrawTopCard()
    {
        if (cards.Count == 0)
            return null;

        return cards.Pop();
    }

    public void RevealTopCard()
    {
        if (cards.Count == 0)
            return;

        Card top = cards.Peek();
        top.Flip(true);
    }

    public int Count
    {
        get { return cards.Count; }
    }

    public void ClearPile()
    {
        while (cards.Count > 0)
        {
            Card card = cards.Pop();
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }
}
