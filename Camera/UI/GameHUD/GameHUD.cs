using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour, UIScreen
{
    private bool ready = false;

    // Game Manager
    private GameManager gameManager;

    // Game UI
    private GameObject joystick;
    private Image joystickSprite;

    private GameObject score;
    private Text scoreText;

    private GameObject pause;
    private Image pauseSprite;
    private Button pauseBtn;

    private PauseScreen pauseScreen;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // Score
        score = transform.Search("Score").gameObject;
        scoreText = score.GetComponent<Text>();

        // Joystick
        joystick = transform.Search("MobileJoystick").gameObject;
        joystickSprite = joystick.GetComponent<Image>();

        // Pause Button
        pause = transform.Search("Pause Button").gameObject;
        pauseSprite = pause.GetComponent<Image>();
        pauseBtn = pause.GetComponent<Button>();
        pauseBtn.onClick.AddListener(PauseClicked);

        // Pause Screen
        pauseScreen = GetComponentInChildren<PauseScreen>();

        ready = true;

        Hide();
    }

    private void PauseClicked()
    {
        Hide();
        pauseScreen.Show();
        gameManager.Pause();
    }

    public void Show()
    {
        scoreText.enabled = true;

        joystickSprite.raycastTarget = true;
        joystickSprite.enabled = true;

        pauseBtn.interactable = true;
        pauseSprite.raycastTarget = true;
        pauseSprite.enabled = true;

        gameManager.Resume();
    }

    public void Hide()
    {
        scoreText.enabled = false;

        joystickSprite.raycastTarget = false;
        joystickSprite.enabled = false;

        pauseBtn.interactable = false;
        pauseSprite.raycastTarget = false;
        pauseSprite.enabled = false;
    }

    public bool isReady()
    {
        return ready;
    }
}
