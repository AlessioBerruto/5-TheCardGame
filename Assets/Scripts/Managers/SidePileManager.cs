using System.Collections.Generic;
using UnityEngine;

public class SidePileManager : MonoBehaviour
{
    [SerializeField] private CoveredPile playerPile;
    [SerializeField] private CoveredPile aiPile;

    public void SetupSidePiles(List<Card> playerCards, List<Card> aiCards)
    {
        playerPile.InitPile(playerCards);
        aiPile.InitPile(aiCards);
    }

    public void RevealPlayerTop()
    {
        playerPile.RevealTopCard();
    }

    public void OnPlayerCardPlayedFromPile()
    {
        playerPile.RevealTopCard();
    }

    public bool IsPlayerPileEmpty()
    {
        return playerPile.Count == 0;
    }

    public void ClearPiles()
    {
        if (playerPile != null)
            playerPile.ClearPile();

        if (aiPile != null)
            aiPile.ClearPile();
    }
}
