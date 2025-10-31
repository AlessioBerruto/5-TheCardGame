using System.Collections.Generic;
using UnityEngine;

public class PlayFieldSlot : MonoBehaviour
{
    private List<Card> cards = new List<Card>();

    public string Suit
    {
        get
        {
            if (cards.Count > 0)
                return cards[0].Suit;
            return "";
        }
    }

    public void InitSlotWithAce(Card ace)
    {
        cards.Add(ace);
        ace.Flip(true);
        ace.transform.SetParent(transform);
        ace.transform.localPosition = Vector3.zero;
    }

    public List<Card> GetCards()
    {
        List<Card> copy = new List<Card>(cards);
        return copy;
    }

    public bool CanPlace(Card card)
    {
        if (cards.Count == 0)
            return card.Rank == 1 || card.IsJoker;

        Card top = cards[cards.Count - 1];

        if (top.IsJoker)
        {
            int nextPlus = GetNextExpectedRank(top, +1);
            int nextMinus = GetNextExpectedRank(top, -1);
            if (card.Rank == nextPlus || card.Rank == nextMinus)
                return true;
        }

        if (card.IsJoker)
            return true;

        if (card.Suit != Suit)
            return false;

        return card.Rank == top.Rank + 1;
    }

    private int GetNextExpectedRank(Card baseCard, int direction)
    {
        int next = baseCard.Rank + direction;
        return Mathf.Clamp(next, 1, 13);
    }

    public void PlaceCard(Card card)
    {
        cards.Add(card);
        card.transform.SetParent(transform);
        card.transform.localPosition = new Vector3(0, cards.Count * 5f, 0);
        card.Flip(true);

        if (JokerManager.Instance != null)
            JokerManager.Instance.CheckForJokerReplacement(this, card);
    }

    public void ClearSlot()
    {
        foreach (Card card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
    }
}
