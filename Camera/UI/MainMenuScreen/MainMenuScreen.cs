using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private const float MENU_TRANSITION_TIME = .75f;

    private Button startNewGameBtn;
    private Image startNewGameBtnSprite;
    private Text startNewGameBtnText;

    private MeshRenderer frontRopesMeshRenderer;

    private GameCamera cam;

    private GameManager gameManager;

    private GameHUD mainGameHUD;

    // Use this for initialization
    void Start()
    {
        mainGameHUD = GameObject.Find("HUD").gameObject.GetComponent<GameHUD>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        cam = Camera.main.GetComponent<GameCamera>();

        GameObject startNewGame = transform.Search("Start New Game Button").gameObject;
        startNewGameBtn = startNewGame.GetComponent<Button>();
        startNewGameBtn.onClick.AddListener(StartNewGame);
        startNewGameBtnSprite = startNewGame.GetComponent<Image>();
        startNewGameBtnText = startNewGame.GetComponentInChildren<Text>();

        frontRopesMeshRenderer = GameObject.Find("ropes.front").GetComponent<MeshRenderer>();

        ready = true;

        Show();
    }

    public void StartNewGame()
    {
        Hide();

        StartCoroutine(cam.ZoomToGameStart(MENU_TRANSITION_TIME));

        Invoke("ShowGameControls", MENU_TRANSITION_TIME);
    }

    private void ShowGameControls()
    {
        gameManager.StartNewGame();
    }

    public void Hide()
    {
        startNewGameBtn.interactable = false;

        startNewGameBtnSprite.raycastTarget = false;
        startNewGameBtnSprite.enabled = false;

        startNewGameBtnText.enabled = false;

        frontRopesMeshRenderer.enabled = false;
    }

    public void Show()
    {
        StartCoroutine(MainMenuSetUp());
        startNewGameBtn.interactable = true;

        startNewGameBtnSprite.raycastTarget = true;
        startNewGameBtnSprite.enabled = true;

        startNewGameBtnText.enabled = true;

        frontRopesMeshRenderer.enabled = true;
    }

    private IEnumerator MainMenuSetUp()
    {
        yield return new WaitUntil(
            () =>
                mainGameHUD.isReady() &&
                cam != null
        );

        gameManager.CleanUpScene();
        cam.SetMainMenuCamera();
    }

    public bool isReady()
    {
        return true;
    }
}
