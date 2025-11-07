using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] GameManager gameManager;
    [SerializeField] DeckManager deckManager;
    [SerializeField] HandManager handManager;
    [SerializeField] SidePileManager sidePileManager;
    [SerializeField] DiscardPile discardPile;

    bool nextReveal;
    bool nextDraw5;
    bool nextIsPlayer;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartTurns()
    {
        Debug.Log("=== INIZIO PARTITA: turno PLAYER ===");
        gameManager.UpdateState(GameManager.GameState.PlayerTurn);
        StartCoroutine(StartTurnFlow(true));
    }

    public void StartPlayerTurn()
    {
        Debug.Log("== Avvio turno PLAYER ==");
        StartCoroutine(StartTurnFlow(true));
    }

    public void StartEnemyTurn()
    {
        Debug.Log("== Avvio turno AI ==");
        gameManager.UpdateState(GameManager.GameState.EnemyTurn);
        StartCoroutine(StartTurnFlow(false));
    }

    IEnumerator StartTurnFlow(bool isPlayer)
    {
        Debug.Log($"[TURN] Avvio turno di {(isPlayer ? "PLAYER" : "AI")}");
        Debug.Log($"[TURN] nextIsPlayer={nextIsPlayer}, nextDraw5={nextDraw5}, nextReveal={nextReveal}");

        bool doDraw5 = nextDraw5 && nextIsPlayer == isPlayer;
        bool doReveal = nextReveal && nextIsPlayer == isPlayer;

        if (doDraw5)
        {
            Debug.Log($"[TURN] {(isPlayer ? "PLAYER" : "AI")} PESCA 5 CARTE");
            if (isPlayer) yield return StartCoroutine(DrawMultipleForPlayer(5));
            else yield return StartCoroutine(DrawMultipleForAI(5));
        }
        else
        {
            Debug.Log($"[TURN] {(isPlayer ? "PLAYER" : "AI")} PESCA 1 CARTA");
            yield return StartCoroutine(DrawCardRoutine(isPlayer));
        }

        if (doReveal)
        {
            Debug.Log($"[TURN] {(isPlayer ? "PLAYER" : "AI")} RIVELA CARTA MAZZETTO");
            if (isPlayer) sidePileManager.RevealPlayerTop();
            else sidePileManager.RevealAITop();
        }

        if (nextIsPlayer == isPlayer)
        {
            Debug.Log($"[TURN] Reset flag per {(isPlayer ? "PLAYER" : "AI")}");
            nextDraw5 = false;
            nextReveal = false;
        }

        if (!isPlayer)
        {
            Debug.Log("[TURN] Avvio routine AI");
            StartCoroutine(AITurnRoutine());
        }
    }

    IEnumerator DrawCardRoutine(bool forPlayer)
    {
        yield return new WaitForSeconds(0.25f);
        Card drawn = deckManager.DrawCard();
        if (drawn == null)
        {
            Debug.Log("[DRAW] Mazzo vuoto, riciclo scarti");
            deckManager.RecycleDiscardPile();
            drawn = deckManager.DrawCard();
            if (drawn == null)
            {
                Debug.Log("[DRAW] Nessuna carta da pescare");
                yield break;
            }
        }
        if (forPlayer)
        {
            Debug.Log("[DRAW] Player pesca una carta");
            handManager.AddCardToPlayer(drawn);
        }
        else
        {
            Debug.Log("[DRAW] AI pesca una carta");
            handManager.AddCardToAI(drawn);
        }
    }

    IEnumerator DrawMultipleForPlayer(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Card drawn = deckManager.DrawCard();
            if (drawn == null)
            {
                Debug.Log("[DRAW] Mazzo vuoto, riciclo scarti");
                deckManager.RecycleDiscardPile();
                drawn = deckManager.DrawCard();
                if (drawn == null) break;
            }
            handManager.AddCardToPlayer(drawn);
        }
        Debug.Log("[DRAW] Player ha completato la pesca multipla");
    }

    IEnumerator DrawMultipleForAI(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Card drawn = deckManager.DrawCard();
            if (drawn == null)
            {
                Debug.Log("[DRAW] Mazzo vuoto, riciclo scarti");
                deckManager.RecycleDiscardPile();
                drawn = deckManager.DrawCard();
                if (drawn == null) break;
            }
            handManager.AddCardToAI(drawn);
        }
        Debug.Log("[DRAW] AI ha completato la pesca multipla");
    }

    IEnumerator AITurnRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        List<Card> aiCards = handManager.AIHand;
        if (aiCards.Count == 0)
        {
            Debug.Log("[AI TURN] Mano AI vuota, fine turno");
            gameManager.EndEnemyTurn(false);
            yield break;
        }

        foreach (Card c in new List<Card>(aiCards))
        {
            PlayFieldSlot[] slots = Object.FindObjectsOfType<PlayFieldSlot>();
            foreach (PlayFieldSlot slot in slots)
            {
                if (slot.CanPlace(c))
                {
                    Debug.Log("[AI TURN] AI gioca carta sul campo");
                    slot.PlaceCard(c, false);
                    handManager.RemoveCard(c);
                    if (handManager.AIHand.Count == 0)
                    {
                        Debug.Log("[AI TURN] AI ha svuotato la mano");
                        sidePileManager.RevealAITop();
                        StartCoroutine(DrawMultipleForAI(5));
                        yield return new WaitForSeconds(0.25f);
                        StartCoroutine(AITurnRoutine());
                        yield break;
                    }
                    yield return new WaitForSeconds(0.6f);
                    AIDiscardMandatory();
                    yield break;
                }
            }
        }

        AIDiscardMandatory();
    }

    void AIDiscardMandatory()
    {
        List<Card> aiCards = handManager.AIHand;
        if (aiCards.Count == 0)
        {
            Debug.Log("[AI DISCARD] AI ha mano vuota, imposta flags");
            nextIsPlayer = false;
            nextReveal = true;
            nextDraw5 = true;
            gameManager.EndEnemyTurn(false);
            return;
        }

        Card chosen = ChooseHighestUnplayableCard(aiCards);
        if (chosen != null)
        {
            Debug.Log("[AI DISCARD] AI scarta una carta");
            discardPile.AddCard(chosen);
            handManager.RemoveCard(chosen);
        }

        if (handManager.AIHand.Count == 0)
        {
            Debug.Log("[AI DISCARD] AI mano vuota dopo scarto, imposta flags");
            nextIsPlayer = false;
            nextReveal = true;
            nextDraw5 = true;
        }

        gameManager.EndEnemyTurn(false);
    }

    Card ChooseHighestUnplayableCard(List<Card> aiCards)
    {
        Card chosen = null;
        int highestRank = -1;
        for (int i = 0; i < aiCards.Count; i++)
        {
            Card card = aiCards[i];
            if (!CanBePlayed(card) && card.Rank > highestRank)
            {
                highestRank = card.Rank;
                chosen = card;
            }
        }
        if (chosen == null && aiCards.Count > 0) chosen = aiCards[Random.Range(0, aiCards.Count)];
        return chosen;
    }

    bool CanBePlayed(Card card)
    {
        PlayFieldSlot[] slots = Object.FindObjectsOfType<PlayFieldSlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].CanPlace(card)) return true;
        }
        return false;
    }

    public void NotifyDiscardedLastCard(bool isPlayer)
    {
        Debug.Log($"[NOTIFY] {(isPlayer ? "PLAYER" : "AI")} ha scartato l’ultima carta → set flags");
        nextIsPlayer = isPlayer;
        nextReveal = true;
        nextDraw5 = true;

        if (isPlayer)
            StartCoroutine(DelayEndPlayerTurn());
        else
            StartCoroutine(DelayEndEnemyTurn());
    }

    IEnumerator DelayEndPlayerTurn()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("[DELAY] Passaggio turno dal PLAYER all’AI");
        gameManager.EndPlayerTurn();
    }

    IEnumerator DelayEndEnemyTurn()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("[DELAY] Passaggio turno dall’AI al PLAYER");
        gameManager.EndEnemyTurn(false);
    }


    public void NotifyPlayedLastCardToField(bool isPlayer)
    {
        Debug.Log($"[NOTIFY] {(isPlayer ? "PLAYER" : "AI")} ha giocato l’ultima carta sul campo");
        if (isPlayer)
        {
            sidePileManager.RevealPlayerTop();
            StartCoroutine(DrawMultipleForPlayer(5));
        }
        else
        {
            sidePileManager.RevealAITop();
            StartCoroutine(DrawMultipleForAI(5));
            StartCoroutine(AITurnRoutine());
        }
    }
}
