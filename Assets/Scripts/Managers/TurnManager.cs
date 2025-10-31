using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private GameManager gameManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private SidePileManager sidePileManager;
    [SerializeField] private DiscardPile discardPile;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartTurns()
    {
        gameManager.UpdateState(GameManager.GameState.PlayerTurn);
        StartCoroutine(DrawCardRoutine(true));
    }

    public void StartEnemyTurn()
    {
        gameManager.UpdateState(GameManager.GameState.EnemyTurn);
        StartCoroutine(DrawCardRoutine(false));
    }

    private IEnumerator DrawCardRoutine(bool forPlayer)
    {
        yield return new WaitForSeconds(0.5f);

        Card drawn = deckManager.DrawCard();
        if (drawn == null) yield break;

        if (forPlayer)
        {
            handManager.AddCardToPlayer(drawn);
            Debug.Log("Giocatore pesca una carta.");
        }
        else
        {
            handManager.AddCardToAI(drawn);
            Debug.Log("IA pesca una carta.");
            yield return new WaitForSeconds(1f);
            StartCoroutine(AITurnRoutine());
        }
    }

    private IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(1f);

        List<Card> aiCards = handManager.AIHand;
        if (aiCards.Count == 0) yield break;

        Card toDiscard = ChooseHighestUnplayableCard(aiCards);
        discardPile.AddCard(toDiscard);
        handManager.RemoveCard(toDiscard);

        Debug.Log($"IA scarta {toDiscard.Suit} {toDiscard.Rank}");
        yield return new WaitForSeconds(1f);

        StartTurns();
    }

    private Card ChooseHighestUnplayableCard(List<Card> aiCards)
    {
        Card chosen = null;
        int highestRank = -1;

        foreach (Card card in aiCards)
        {
            if (!CanBePlayed(card) && card.Rank > highestRank)
            {
                highestRank = card.Rank;
                chosen = card;
            }
        }

        return chosen ?? aiCards[Random.Range(0, aiCards.Count)];
    }

    private bool CanBePlayed(Card card)
    {
        foreach (var slot in FindObjectsOfType<PlayFieldSlot>())
        {
            if (slot.CanPlace(card))
                return true;
        }
        return false;
    }
}
