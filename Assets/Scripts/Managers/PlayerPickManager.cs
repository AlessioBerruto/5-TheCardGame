using UnityEngine;
using System.Collections.Generic;

public class PlayerPickManager : MonoBehaviour
{
    [SerializeField] private PlayFieldManager playFieldManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject selectorPrefab; 

    private List<List<Card>> previewPiles = new List<List<Card>>();
    private bool isSelectionDone = false;
    private List<GameObject> activeSelectors = new List<GameObject>();

    public void StartPileSelection(List<List<Card>> piles)
    {
        previewPiles = piles;
        isSelectionDone = false;
        uiManager.ShowCardsCanvas();

        Debug.Log("Fase di scelta: clicca sul mazzetto che vuoi per la pila coperta!");
        CreateSelectors();
    }

    private void CreateSelectors()
    {
        ClearSelectors();

        List<CoveredPile> piles = playFieldManager.GetPreviewPiles();

        for (int i = 0; i < piles.Count; i++)
        {
            CoveredPile pile = piles[i];
            if (pile == null) continue;

            GameObject selector = Instantiate(selectorPrefab, pile.transform);
            PileSelector selectorScript = selector.GetComponent<PileSelector>();
            selectorScript.pileIndex = i;


            activeSelectors.Add(selector);
        }
    }

    public void OnPlayerSelectPile(int index)
    {
        if (isSelectionDone) return;
        isSelectionDone = true;

        ClearSelectors();

        int aiIndex = Random.Range(0, 4);
        while (aiIndex == index)
            aiIndex = Random.Range(0, 4);

        Debug.Log("Giocatore ha scelto pila " + index + ", IA ha scelto pila " + aiIndex);

        List<int> remaining = new List<int> { 0, 1, 2, 3 };
        remaining.Remove(index);
        remaining.Remove(aiIndex);

        DeckManager deckManager = FindObjectOfType<DeckManager>();
        if (deckManager != null)
        {
            deckManager.CompleteSetupAfterChoice(
                playerPileIndex: index,
                aiPileIndex: aiIndex,
                playerHandIndex: remaining[0],
                aiHandIndex: remaining[1]
            );
        }
        else
        {
            Debug.LogWarning("DeckManager non trovato!");
        }
    }

    private void ClearSelectors()
    {
        foreach (GameObject sel in activeSelectors)
        {
            if (sel != null)
                Destroy(sel);
        }
        activeSelectors.Clear();
    }
}
