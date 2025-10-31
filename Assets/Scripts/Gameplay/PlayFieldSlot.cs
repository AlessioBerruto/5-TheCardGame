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

    public void InitSlotWithAce(Card ace, bool isFacingPlayer = true)
    {
        if (ace == null) return;
        cards.Add(ace);
        ace.Flip(true);
        ace.transform.SetParent(transform, false);
        ace.transform.localScale = new Vector3(1, isFacingPlayer ? 1 : -1, 1);
        ace.transform.localPosition = Vector3.zero;
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

    public void PlaceCard(Card card, bool isFacingPlayer = true)
    {
        cards.Add(card);
        card.transform.SetParent(transform, false);

        float yOffset = cards.Count * 5f;
        if (!isFacingPlayer) yOffset = -yOffset;

        card.transform.localPosition = new Vector3(0, yOffset, 0);
        card.transform.localScale = new Vector3(1, isFacingPlayer ? 1 : -1, 1);
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

    public List<Card> GetCards()
    {
        return new List<Card>(cards);
    }
}
