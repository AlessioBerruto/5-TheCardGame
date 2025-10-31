using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private PlayFieldManager playFieldManager;
    [SerializeField] private PlayerPickManager playerPickManager;
    [SerializeField] private TurnManager turnManager;

    private List<Card> mainDeck = new List<Card>();
    private List<List<Card>> previewPiles = new List<List<Card>>();
    private List<Card> cachedAces = new List<Card>();

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

        Debug.Log("DeckManager: 4 mazzetti creati e pronti per la selezione.");
    }

    public void CompleteSetupAfterChoice(int playerPileIndex, int aiPileIndex, int playerHandIndex, int aiHandIndex)
    {
        List<Card> playerPile = previewPiles[playerPileIndex];
        List<Card> aiPile = previewPiles[aiPileIndex];
        List<Card> playerHand = previewPiles[playerHandIndex];
        List<Card> aiHand = previewPiles[aiHandIndex];

        playFieldManager.PlaceStartingAces(cachedAces);
        playFieldManager.InitHands(playerHand, aiHand);
        playFieldManager.InitSidePiles(playerPile, aiPile);
        playFieldManager.InitDeck(mainDeck);
        playFieldManager.InitDiscardPile();

        Debug.Log("DeckManager: setup completato, avvio turni.");

        GameManager.Instance.UpdateState(GameManager.GameState.PlayerTurn);
        turnManager.StartTurns();
    }

    public Card DrawCard()
    {
        if (mainDeck.Count == 0)
        {
            Debug.LogWarning("DeckManager: mazzo principale vuoto!");
            return null;
        }

        Card drawn = mainDeck[0];
        mainDeck.RemoveAt(0);
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
}
