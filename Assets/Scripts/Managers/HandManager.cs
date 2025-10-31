using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform playerHandZone;
    [SerializeField] private Transform aiHandZone;
    [SerializeField] private float cardSpacing = 100f;
    [SerializeField] private float rotationAmount = 8f;
    [SerializeField] private float maxYOffset = 20f;

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
        if (card == null) return;

        PlayerHand.Add(card);
        ArrangeHand(PlayerHand, playerHandZone, true);
    }

    public void AddCardToAI(Card card)
    {
        if (card == null) return;

        AIHand.Add(card);
        ArrangeHand(AIHand, aiHandZone, false);
    }

    public void RemoveCard(Card card)
    {
        if (PlayerHand.Contains(card))
        {
            PlayerHand.Remove(card);
            ArrangeHand(PlayerHand, playerHandZone, true);
        }
        else if (AIHand.Contains(card))
        {
            AIHand.Remove(card);
            ArrangeHand(AIHand, aiHandZone, false);
        }
    }

    public void ClearHands()
    {
        for (int i = 0; i < PlayerHand.Count; i++)
            Destroy(PlayerHand[i].gameObject);

        for (int i = 0; i < AIHand.Count; i++)
            Destroy(AIHand[i].gameObject);

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
            c.transform.SetParent(zone, false);
            c.gameObject.SetActive(true);

            float offsetX = (i - centerIndex) * (cardSpacing * 0.6f);
            float rotationZ = (i - centerIndex) * (isPlayer ? -rotationAmount : rotationAmount);

            float distanceFromCenter = Mathf.Abs(i - centerIndex);
            float offsetY = maxYOffset - (distanceFromCenter / centerIndex) * maxYOffset;
            offsetY = Mathf.Clamp(offsetY, 0, maxYOffset);

            if (!isPlayer)
            {
                offsetY = maxYOffset - offsetY;
            }

            c.transform.localPosition = new Vector3(offsetX, offsetY, 0);
            c.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);

            c.Flip(isPlayer);

            CardDragHandler dragHandler = c.GetComponent<CardDragHandler>();
            if (dragHandler != null)
                dragHandler.SetDraggable(true);
        }
    }
}
