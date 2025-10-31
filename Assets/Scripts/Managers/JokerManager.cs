using System.Collections.Generic;
using UnityEngine;

public class JokerManager : MonoBehaviour
{
    public static JokerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void CheckForJokerReplacement(PlayFieldSlot slot, Card playedCard)
    {
        List<Card> cardsInSlot = slot.GetCards();

        for (int i = 0; i < cardsInSlot.Count - 1; i++)
        {
            Card c = cardsInSlot[i];

            if (c.IsJoker)
            {
                if (playedCard.Suit == slot.Suit &&
                    (playedCard.Rank == c.Rank + 1 || playedCard.Rank == c.Rank - 1))
                {
                    ReturnJokerToOwner(c, playedCard);
                }
            }
        }
    }

    private void ReturnJokerToOwner(Card joker, Card playedCard)
    {
        joker.transform.SetParent(null);
        joker.gameObject.SetActive(false);

        HandManager handManager = FindObjectOfType<HandManager>();

        if (playedCard.transform.IsChildOf(handManager.transform.Find("PlayerHandZone")))
        {
            handManager.AddCardToPlayer(joker);
        }
        else
        {
            handManager.AddCardToAI(joker);
        }
    }

    public void CheckForJokerReturn(Card playedCard)
    {
        if (playedCard.IsJoker) return;

        PlayFieldSlot[] slots = FindObjectsOfType<PlayFieldSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            List<Card> slotCards = slots[i].GetCards();
            for (int j = 0; j < slotCards.Count; j++)
            {
                if (slotCards[j].IsJoker && slotCards[j].Suit == playedCard.Suit)
                {
                    ReturnJokerToOwner(slotCards[j], playedCard);
                    return;
                }
            }
        }
    }
}
