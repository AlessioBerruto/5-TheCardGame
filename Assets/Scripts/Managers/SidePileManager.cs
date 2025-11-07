using System.Collections.Generic;
using UnityEngine;

public class SidePileManager : MonoBehaviour
{
    [SerializeField] CoveredPile playerPile;
    [SerializeField] CoveredPile aiPile;

    public void SetupSidePiles(List<Card> playerCards, List<Card> aiCards)
    {
        playerPile.InitPile(playerCards);
        aiPile.InitPile(aiCards);
    }

    public void RevealPlayerTop()
    {
        if (!playerPile.IsEmpty)
            playerPile.RevealTopCard();
    }

    public void RevealAITop()
    {
        if (!aiPile.IsEmpty)
            aiPile.RevealTopCard();
    }

    public void OnPlayerCardPlayedFromPile()
    {
        if (!playerPile.IsEmpty)
            playerPile.RevealTopCard();
    }

    public void OnAICardPlayedFromPile()
    {
        if (!aiPile.IsEmpty)
            aiPile.RevealTopCard();
    }

    public bool IsPlayerPileEmpty()
    {
        return playerPile.IsEmpty;
    }

    public bool IsAIPileEmpty()
    {
        return aiPile.IsEmpty;
    }

    public void ClearPiles()
    {
        if (playerPile != null)
            playerPile.ClearPile();
        if (aiPile != null)
            aiPile.ClearPile();
    }
}
