using System.Collections.Generic;
using UnityEngine;

public class PlayFieldManager : MonoBehaviour
{
    [SerializeField] private List<PlayFieldSlot> fieldSlots = new List<PlayFieldSlot>();
    [SerializeField] private List<CoveredPile> previewPiles = new List<CoveredPile>();
    [SerializeField] private SidePileManager sidePileManager;
    [SerializeField] private DiscardPile discardPile;
    [SerializeField] private HandManager handManager;

    public void ShowPreviewPiles(List<List<Card>> piles)
    {
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
    }

    public void PlaceStartingAces(List<Card> aces)
    {
        List<Card> remaining = new List<Card>(aces);

        for (int i = 0; i < fieldSlots.Count; i++)
        {
            PlayFieldSlot slot = fieldSlots[i];
            string slotName = slot.gameObject.name.ToLower();
            Card chosen = null;

            for (int j = 0; j < remaining.Count; j++)
            {
                if (remaining[j] == null) continue;
                string suit = remaining[j].Suit.ToLower();

                if (slotName.Contains(suit) || slotName.Contains(suit + "s"))
                {
                    chosen = remaining[j];
                    remaining.RemoveAt(j);
                    break;
                }
            }

            if (chosen == null && remaining.Count > 0)
            {
                chosen = remaining[0];
                remaining.RemoveAt(0);
            }

            if (chosen != null)
            {
                bool isTopRow = slot.transform.position.y > 0f;

                chosen.transform.SetParent(slot.transform, false);
                chosen.transform.localScale = Vector3.one;
                chosen.transform.localPosition = Vector3.zero;
                chosen.Flip(true);

                if (isTopRow)
                    chosen.transform.localRotation = Quaternion.Euler(0, 0, 180);
                else
                    chosen.transform.localRotation = Quaternion.identity;

                chosen.gameObject.SetActive(true);
                slot.InitSlotWithAce(chosen, !isTopRow);
            }
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

    public void InitDiscardPile()
    {
        discardPile.ClearPile();
    }

    public void ClearField()
    {
        foreach (PlayFieldSlot slot in fieldSlots)
            slot.ClearSlot();

        discardPile.ClearPile();
        sidePileManager.ClearPiles();
    }

    public List<CoveredPile> GetPreviewPiles()
    {
        return previewPiles;
    }
}
