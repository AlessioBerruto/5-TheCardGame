using System.Collections.Generic;
using UnityEngine;

public class PlayFieldSlot : MonoBehaviour
{
    List<Card> cards = new List<Card>();
    bool facingPlayer;

    public string Suit
    {
        get
        {
            if (cards.Count > 0)
                return cards[0].Suit;
            return "";
        }
    }

    public void InitSlotWithAce(Card ace, bool facePlayer)
    {
        cards.Clear();
        facingPlayer = facePlayer;
        cards.Add(ace);
        ace.Flip(true);
        ace.transform.SetParent(transform, false);
        ace.transform.localPosition = Vector3.zero;
        ace.transform.localRotation = facePlayer ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
    }

    public bool CanPlace(Card card)
    {
        if (cards.Count == 0)
            return card.Rank == 1 || card.IsJoker;

        Card top = cards[cards.Count - 1];
        if (card.IsJoker) return true;
        if (top.IsJoker) return true;
        if (card.Suit != Suit) return false;
        return card.Rank == top.Rank + 1;
    }

    public void PlaceCard(Card card, bool fromPlayer)
    {
        cards.Add(card);
        card.transform.SetParent(transform, false);

        float offset = 20f;
        float direction = facingPlayer ? -1f : 1f;
        card.transform.localPosition = new Vector3(0, cards.Count * offset * direction, 0);

        bool shouldFacePlayer = facingPlayer ? true : false;
        card.transform.localRotation = shouldFacePlayer ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
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
