using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] DeckManager deckManager;
    [SerializeField] PlayFieldManager playFieldManager;
    [SerializeField] SidePileManager sidePileManager;
    [SerializeField] HandManager handManager;
    [SerializeField] UIManager uiManager;

    public enum GameState { None, Setup, ChoosingPile, PlayerTurn, EnemyTurn, GameOver }
    public GameState CurrentState { get; private set; } = GameState.None;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        uiManager.ShowMainMenu();
    }

    public void StartGame()
    {
        CurrentState = GameState.Setup;
        uiManager.ShowCardsCanvas();
        deckManager.SetupDeck();
        CurrentState = GameState.ChoosingPile;
    }

    public void NewGame()
    {
        playFieldManager.ClearField();
        handManager.ClearHands();
        sidePileManager.ClearPiles();
        StartGame();
    }

    public void UpdateState(GameState newState)
    {
        CurrentState = newState;
    }

    public void EndPlayerTurn()
    {
        if (sidePileManager.IsPlayerPileEmpty())
        {
            EndGame(true);
            return;
        }
        CurrentState = GameState.EnemyTurn;
        TurnManager.Instance.StartEnemyTurn();
    }

    public void EndEnemyTurn(bool revealNext)
    {
        if (sidePileManager.IsAIPileEmpty())
        {
            EndGame(false);
            return;
        }
        CurrentState = GameState.PlayerTurn;
        TurnManager.Instance.StartPlayerTurn();
    }

    public void EndGame(bool playerWon)
    {
        CurrentState = GameState.GameOver;
        if (playerWon) uiManager.OnGameWon();
        else uiManager.OnGameLost();
    }

    public void OnPlayerDiscardLastCard()
    {
        TurnManager.Instance.NotifyDiscardedLastCard(true);
    }

    public void OnPlayerPlayedPileCard()
    {
        TurnManager.Instance.NotifyPlayedLastCardToField(true);
    }

    public void OnAIDiscardLastCard()
    {
        TurnManager.Instance.NotifyDiscardedLastCard(false);
    }

    public void OnAIPlayedPileCard()
    {
        TurnManager.Instance.NotifyPlayedLastCardToField(false);
    }
}
