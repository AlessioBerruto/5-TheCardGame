using System.Collections.Generic;
using UnityEngine;

public class CoveredPile : MonoBehaviour
{
    [SerializeField] bool isMainDeck;
    [SerializeField] bool isPlayerPile;

    List<Card> pile = new List<Card>();
    Card revealedTop;

    public int Count => pile.Count;
    public bool IsEmpty => pile.Count == 0;

    public void InitPile(List<Card> cards)
    {
        ClearPile();
        pile.AddRange(cards);
        for (int i = 0; i < pile.Count; i++)
        {
            Card c = pile[i];
            c.gameObject.SetActive(true);
            c.transform.SetParent(transform, false);
            c.transform.localScale = Vector3.one;
            c.transform.localRotation = Quaternion.identity;
            c.transform.localPosition = new Vector3(0, i * 2f, 0);
            c.Flip(false);
        }
        if (!isMainDeck && pile.Count > 0)
        {
            revealedTop = null;
            pile[pile.Count - 1].gameObject.SetActive(true);
        }
    }

    public void RevealTopCard()
    {
        if (pile.Count == 0) return;

        if (revealedTop != null && pile.Contains(revealedTop))
            pile.Remove(revealedTop);

        revealedTop = pile[pile.Count - 1];
        revealedTop.Flip(true);
        revealedTop.transform.SetParent(transform, false);
        revealedTop.transform.localScale = Vector3.one;

        if (isPlayerPile)
            revealedTop.transform.localRotation = Quaternion.identity;          
        else
            revealedTop.transform.localRotation = Quaternion.Euler(0, 0, 180);  

        revealedTop.transform.localPosition = Vector3.zero;
        revealedTop.gameObject.SetActive(true);
    }


    public Card DrawTopCard()
    {
        if (pile.Count == 0) return null;
        Card c = pile[pile.Count - 1];
        pile.RemoveAt(pile.Count - 1);
        revealedTop = pile.Count > 0 ? pile[pile.Count - 1] : null;
        if (revealedTop != null)
        {
            revealedTop.Flip(true);
            revealedTop.transform.localScale = Vector3.one;
            revealedTop.transform.localRotation = isPlayerPile ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
            revealedTop.transform.localPosition = Vector3.zero;
            revealedTop.gameObject.SetActive(true);
        }
        return c;
    }

    public void ClearPile()
    {
        for (int i = 0; i < pile.Count; i++)
        {
            if (pile[i] != null) Destroy(pile[i].gameObject);
        }
        pile.Clear();
        revealedTop = null;
    }
}
