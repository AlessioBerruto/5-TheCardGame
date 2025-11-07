using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] PlayFieldManager playFieldManager;
    [SerializeField] PlayerPickManager playerPickManager;
    [SerializeField] TurnManager turnManager;
    [SerializeField] Transform mainDeckVisualParent;
    [SerializeField] int maxVisibleDeckCards = 20;
    [SerializeField] float cardOffsetY = -2f;

    List<Card> mainDeck = new List<Card>();
    List<List<Card>> previewPiles = new List<List<Card>>();
    List<Card> cachedAces = new List<Card>();

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

        HideMainDeckVisual();
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

        ShowMainDeckVisual();
        RefreshDeckVisual();

        GameManager.Instance.UpdateState(GameManager.GameState.PlayerTurn);
        turnManager.StartTurns();
    }

    void HideMainDeckVisual()
    {
        if (mainDeckVisualParent != null) mainDeckVisualParent.gameObject.SetActive(false);
    }

    void ShowMainDeckVisual()
    {
        if (mainDeckVisualParent != null) mainDeckVisualParent.gameObject.SetActive(true);
    }

    public Card DrawCard()
    {
        if (mainDeck.Count == 0)
        {
            Debug.LogWarning("DeckManager: mazzo principale vuoto!");
            return null;
        }
        Card drawn = mainDeck[mainDeck.Count - 1];
        mainDeck.RemoveAt(mainDeck.Count - 1);
        RefreshDeckVisual();
        return drawn;
    }

    void Shuffle(List<Card> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Card temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void RefreshDeckVisual()
    {
        if (mainDeckVisualParent == null) return;
        int visibleCount = Mathf.Min(mainDeck.Count, maxVisibleDeckCards);

        for (int i = 0; i < mainDeck.Count; i++)
        {
            Card c = mainDeck[i];
            if (c == null) continue;
            c.transform.SetParent(mainDeckVisualParent, false);
            c.transform.localRotation = Quaternion.Euler(0, 0, 180);
            c.transform.localScale = Vector3.one;
            c.transform.localPosition = Vector3.zero;
            c.transform.SetSiblingIndex(i);
            bool show = i >= mainDeck.Count - visibleCount;
            if (show)
            {
                int layerIndexFromTop = (mainDeck.Count - 1) - i;
                c.transform.localPosition = new Vector3(0, -layerIndexFromTop * cardOffsetY, 0);
                c.gameObject.SetActive(true);
                c.Flip(false);
            }
            else c.gameObject.SetActive(false);
        }
    }


    public void RecycleDiscardPile()
    {
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        if (discardPile == null) return;

        List<Card> discardedCards = discardPile.GetAllCards();
        if (discardedCards.Count == 0) return;

        discardPile.ClearPile();

        for (int i = 0; i < discardedCards.Count; i++)
        {
            Card c = discardedCards[i];
            c.Flip(false);
            c.transform.SetParent(mainDeckVisualParent, false);
            c.transform.localRotation = Quaternion.Euler(0, 0, 180);
            c.transform.localScale = Vector3.one;
            c.transform.localPosition = Vector3.zero;
            c.gameObject.SetActive(false);
            mainDeck.Add(c);
        }

        Shuffle(mainDeck);
        RefreshDeckVisual();
        Debug.Log("DeckManager: scarti riciclati nel mazzo principale (" + discardedCards.Count + " carte).");
    }
}
