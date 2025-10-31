using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private DeckManager deckManager;
    [SerializeField] private PlayFieldManager playFieldManager;
    [SerializeField] private SidePileManager sidePileManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private UIManager uiManager;

    public enum GameState { None, Setup, ChoosingPile, PlayerTurn, EnemyTurn, GameOver }
    public GameState CurrentState { get; private set; } = GameState.None;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        uiManager.ShowMainMenu();
    }

    public void StartGame()
    {
        CurrentState = GameState.Setup;
        uiManager.ShowCardsCanvas();

        Debug.Log("GameManager: inizializzazione mazzo e preparazione mazzetti...");
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
        CurrentState = GameState.EnemyTurn;
        turnManager.StartEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        CurrentState = GameState.PlayerTurn;
        turnManager.StartTurns();
    }

    public void EndGame(bool playerWon)
    {
        CurrentState = GameState.GameOver;

        if (playerWon)
            uiManager.OnGameWon();
        else
            uiManager.OnGameLost();
    }

    public void OnPlayerDiscardLastCard()
    {
        sidePileManager.RevealPlayerTop();
    }

    public void OnPlayerPlayedPileCard()
    {
        sidePileManager.OnPlayerCardPlayedFromPile();

        if (sidePileManager.IsPlayerPileEmpty())
            EndGame(true);
    }
}
