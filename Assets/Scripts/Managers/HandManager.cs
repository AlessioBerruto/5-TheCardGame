using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform playerHandZone;
    [SerializeField] private Transform aiHandZone;
    [SerializeField] private float cardSpacing = 100f;
    [SerializeField] private float rotationAmount = 8f;

    public List<Card> PlayerHand { get; private set; } = new List<Card>();
    public List<Card> AIHand { get; private set; } = new List<Card>();

    public void InitHands(List<Card> playerCards, List<Card> aiCards)
    {
        PlayerHand = new List<Card>(playerCards);
        AIHand = new List<Card>(aiCards);

        ArrangeHand(PlayerHand, playerHandZone, true);
        ArrangeHand(AIHand, aiHandZone, false);
    }

    public void AddCardToPlayer(Card card)
    {
        PlayerHand.Add(card);
        ArrangeHand(PlayerHand, playerHandZone, true);
    }

    public void AddCardToAI(Card card)
    {
        AIHand.Add(card);
        ArrangeHand(AIHand, aiHandZone, false);
    }

    public void RemoveCard(Card card)
    {
        if (PlayerHand.Remove(card))
            ArrangeHand(PlayerHand, playerHandZone, true);
        else if (AIHand.Remove(card))
            ArrangeHand(AIHand, aiHandZone, false);
    }

    public void ClearHands()
    {
        foreach (Card c in PlayerHand) Destroy(c.gameObject);
        foreach (Card c in AIHand) Destroy(c.gameObject);
        PlayerHand.Clear();
        AIHand.Clear();
    }

    private void ArrangeHand(List<Card> hand, Transform zone, bool isPlayer)
    {
        if (hand == null || hand.Count == 0) return;

        float centerIndex = (hand.Count - 1) / 2f;
        for (int i = 0; i < hand.Count; i++)
        {
            Card c = hand[i];
            c.transform.SetParent(zone);
            c.gameObject.SetActive(true);

            float offsetX = (i - centerIndex) * (cardSpacing * 0.6f);
            float rotationZ = (i - centerIndex) * (isPlayer ? -rotationAmount : rotationAmount);

            c.transform.localPosition = new Vector3(offsetX, 0, 0);
            c.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
            c.Flip(isPlayer);
        }
    }
}
