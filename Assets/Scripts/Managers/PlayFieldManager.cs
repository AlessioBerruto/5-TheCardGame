using System.Collections.Generic;
using UnityEngine;

public class PlayFieldManager : MonoBehaviour
{
    [SerializeField] private List<PlayFieldSlot> fieldSlots = new List<PlayFieldSlot>();
    [SerializeField] private List<CoveredPile> previewPiles = new List<CoveredPile>();
    [SerializeField] private CoveredPile mainDeck;
    [SerializeField] private SidePileManager sidePileManager;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private HandManager handManager;

    public void ShowPreviewPiles(List<List<Card>> piles)
    {
        if (mainDeck != null)
            mainDeck.gameObject.SetActive(false);

        if (previewPiles.Count < piles.Count)
        {
            Debug.LogWarning("Numero di previewPiles inferiore al numero di mazzetti!");
            return;
        }

        for (int i = 0; i < piles.Count; i++)
        {
            CoveredPile pileZone = previewPiles[i];
            pileZone.gameObject.SetActive(true);
            pileZone.InitPile(piles[i]);
            pileZone.transform.SetAsLastSibling();
        }

        Debug.Log("PlayFieldManager: mazzetti di scelta mostrati correttamente.");
    }

    public void PlaceStartingAces(List<Card> aces)
    {
        for (int i = 0; i < fieldSlots.Count && i < aces.Count; i++)
        {
            fieldSlots[i].InitSlotWithAce(aces[i]);
        }
    }

    public void InitHands(List<Card> playerCards, List<Card> aiCards)
    {
        handManager.InitHands(playerCards, aiCards);
    }

    public void InitSidePiles(List<Card> playerPile, List<Card> aiPile)
    {
        sidePileManager.SetupSidePiles(playerPile, aiPile);
    }

    public void InitDeck(List<Card> deckCards)
    {
        mainDeck.InitPile(deckCards);
        mainDeck.gameObject.SetActive(true);
        mainDeck.transform.SetAsLastSibling();
    }

    public void InitDiscardPile()
    {
        discardPile.ClearPile();
    }

    public void ClearField()
    {
        foreach (PlayFieldSlot slot in fieldSlots)
            slot.ClearSlot();

        mainDeck.ClearPile();
        discardPile.ClearPile();
        sidePileManager.ClearPiles();
    }

    public List<CoveredPile> GetPreviewPiles()
    {
        return previewPiles;
    }

}
