using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private PlayFieldManager playFieldManager;
    [SerializeField] private PlayerPickManager playerPickManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Transform mainDeckVisualParent;
    [SerializeField] private int maxVisibleDeckCards = 20;
    [SerializeField] private float cardOffsetY = 2f;

    private List<Card> mainDeck = new List<Card>();
    private List<List<Card>> previewPiles = new List<List<Card>>();
    private List<Card> cachedAces = new List<Card>();
    private List<Card> visibleCards = new List<Card>();

    public void SetupDeck()
    {
        CardPoolManager pool = CardPoolManager.Instance;
        cachedAces = pool.GetAces();

        List<Card> allCards = pool.GetAllNonAceCards();
        Shuffle(allCards);

        previewPiles.Clear();
        for (int i = 0; i < 4; i++)
        {
            List<Card> pile = allCards.GetRange(i * 5, 5);
            previewPiles.Add(pile);
        }

        allCards.RemoveRange(0, 20);
        mainDeck = new List<Card>(allCards);

        playFieldManager.ShowPreviewPiles(previewPiles);
        playerPickManager.StartPileSelection(previewPiles);

        ClearMainDeckVisual();
    }

    public void CompleteSetupAfterChoice(int playerPileIndex, int aiPileIndex, int playerHandIndex, int aiHandIndex)
    {
        List<Card> playerPile = new List<Card>(previewPiles[playerPileIndex]);
        List<Card> aiPile = new List<Card>(previewPiles[aiPileIndex]);
        List<Card> playerHand = new List<Card>(previewPiles[playerHandIndex]);
        List<Card> aiHand = new List<Card>(previewPiles[aiHandIndex]);

        playFieldManager.PlaceStartingAces(cachedAces);
        playFieldManager.InitHands(playerHand, aiHand);
        playFieldManager.InitSidePiles(playerPile, aiPile);

        RefreshDeckVisual();

        GameManager.Instance.UpdateState(GameManager.GameState.PlayerTurn);
        turnManager.StartTurns();
    }

    public Card DrawCard()
    {
        if (mainDeck.Count == 0) return null;

        Card drawn = mainDeck[mainDeck.Count - 1];
        mainDeck.RemoveAt(mainDeck.Count - 1);
        visibleCards.Remove(drawn);

        RefreshDeckVisual();
        return drawn;
    }

    private void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ClearMainDeckVisual()
    {
        foreach (Card c in visibleCards)
        {
            if (c != null) c.gameObject.SetActive(false);
        }
        visibleCards.Clear();
    }

    private void RefreshDeckVisual()
    {
        int visibleCount = Mathf.Min(mainDeck.Count, maxVisibleDeckCards);

        for (int i = 0; i < visibleCount; i++)
        {
            Card c = mainDeck[mainDeck.Count - 1 - i];
            if (!visibleCards.Contains(c)) visibleCards.Add(c);

            c.transform.SetParent(mainDeckVisualParent, false);
            c.transform.localPosition = new Vector3(0, i * cardOffsetY, 0);
            c.transform.localRotation = Quaternion.identity;
            c.Flip(false);
            c.gameObject.SetActive(true);
        }

        for (int i = visibleCards.Count - 1; i >= visibleCount; i--)
        {
            Card c = visibleCards[i];
            c.gameObject.SetActive(false);
            visibleCards.RemoveAt(i);
        }
    }
}
