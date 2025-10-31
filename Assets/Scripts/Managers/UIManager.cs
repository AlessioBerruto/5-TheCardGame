using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject winMenuCanvas;
    [SerializeField] private GameObject loseMenuCanvas;
    [SerializeField] private GameObject cardsCanvas;
    [SerializeField] private GameObject optionsMenuCanvas;

    private GameObject currentMenu;

    private void Start()
    {
        ShowMainMenu();
    }

    private void HideAllMenus()
    {
        mainMenuCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        winMenuCanvas.SetActive(false);
        loseMenuCanvas.SetActive(false);
        cardsCanvas.SetActive(false);        
        optionsMenuCanvas.SetActive(false);
    }

    public void ShowMainMenu()
    {
        HideAllMenus();
        mainMenuCanvas.SetActive(true);
        currentMenu = mainMenuCanvas;
    }
    public void ShowOptionsMenu()
    {
        HideAllMenus();
        optionsMenuCanvas.SetActive(true);
        currentMenu = optionsMenuCanvas;
    }
    public void ShowCardsCanvas()
    {
        HideAllMenus();
        cardsCanvas.SetActive(true);
        currentMenu = cardsCanvas;
    }

    public void OnStartGame()
    {
        HideAllMenus();
        cardsCanvas.SetActive(true);
        Time.timeScale = 1f;
        GameManager.Instance.StartGame();
    }

    public void OnNewGame()
    {
        HideAllMenus();
        cardsCanvas.SetActive(true);
        Time.timeScale = 1f;
        GameManager.Instance.NewGame();
    }

    public void OnPauseGame()
    {
        HideAllMenus();
        pauseMenuCanvas.SetActive(true);
        currentMenu = pauseMenuCanvas;
        Time.timeScale = 0f;
    }

    public void OnResumeGame()
    {
        ShowCardsCanvas();
        Time.timeScale = 1f;
    }

    public void OnGameWon()
    {
        HideAllMenus();
        winMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnGameLost()
    {
        HideAllMenus();
        loseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnExitGame()
    {
        Application.Quit();
        Debug.Log("Gioco chiuso");
    }
}
