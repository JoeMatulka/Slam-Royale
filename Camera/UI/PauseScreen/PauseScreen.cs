using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private GameHUD gameHUD;
    private HowToPlayScreen howToPlayScreen;
    private MainMenuScreen mainMenuScreen;
    private CreditsScreen creditsScreen;

    private Text pauseText;

    private GameObject resume;
    private Image resumeSprite;
    private Text resumeText;
    private Button resumeBtn;

    private GameObject howToPlay;
    private Image howToPlaySprite;
    private Text howToPlayText;
    private Button howToPlayBtn;

    private GameObject sound;
    private Image soundprite;
    private Text soundText;
    private const string SOUND_ON_TEXT = "SOUND: ON";
    private const string SOUND_OFF_TEXT = "SOUND: OFF";
    private Button soundBtn;

    private GameObject credits;
    private Image creditsSprite;
    private Text creditsText;
    private Button creditsBtn;

    private GameObject quit;
    private Image quitSprite;
    private Text quitText;
    private Button quitBtn;

    private Image blanket;

    // Use this for initialization
    void Start()
    {
        gameHUD = GetComponentInParent<GameHUD>();
        howToPlayScreen = GameObject.Find("How To Play Screen").GetComponent<HowToPlayScreen>();
        mainMenuScreen = GameObject.Find("Main Menu Screen").GetComponent<MainMenuScreen>();
        creditsScreen = GameObject.Find("Credits Screen").GetComponent<CreditsScreen>();

        // Title Screen Text
        pauseText = GetComponentInChildren<Text>();

        // Resume Button
        resume = transform.Search("Resume Button").gameObject;
        resumeSprite = resume.GetComponent<Image>();
        resumeBtn = resume.GetComponent<Button>();
        resumeText = resumeBtn.GetComponentInChildren<Text>();
        resumeBtn.onClick.AddListener(ResumeClicked);

        // How to Play Button
        howToPlay = transform.Search("How To Play Button").gameObject;
        howToPlaySprite = howToPlay.GetComponent<Image>();
        howToPlayBtn = howToPlay.GetComponent<Button>();
        howToPlayText = howToPlayBtn.GetComponentInChildren<Text>();
        howToPlayBtn.onClick.AddListener(HowToPlayClicked);

        // Sound Toggle Button
        sound = transform.Search("Sound Button").gameObject;
        soundprite = sound.GetComponent<Image>();
        soundBtn = sound.GetComponent<Button>();
        soundText = soundBtn.GetComponentInChildren<Text>();
        soundText.text = GameDataManager.Instance.IsSound ? SOUND_ON_TEXT : SOUND_OFF_TEXT;
        soundBtn.onClick.AddListener(SoundToggleClicked);

        // Credits Button
        credits = transform.Search("Credits Button").gameObject;
        creditsSprite = credits.GetComponent<Image>();
        creditsBtn = credits.GetComponent<Button>();
        creditsText = creditsBtn.GetComponentInChildren<Text>();
        creditsBtn.onClick.AddListener(CreditsClick);

        // Quit Button
        quit = transform.Search("Quit Button").gameObject;
        quitSprite = quit.GetComponent<Image>();
        quitBtn = quit.GetComponent<Button>();
        quitText = quitBtn.GetComponentInChildren<Text>();
        quitBtn.onClick.AddListener(QuitClicked);

        blanket = transform.Search("Blanket").GetComponent<Image>();

        ready = true;

        Hide();
    }

    public void Show()
    {

        pauseText.enabled = true;

        resumeSprite.enabled = true;
        resumeSprite.raycastTarget = true;
        resumeBtn.interactable = true;
        resumeText.enabled = true;

        howToPlaySprite.enabled = true;
        howToPlaySprite.raycastTarget = true;
        howToPlayBtn.interactable = true;
        howToPlayText.enabled = true;

        soundprite.enabled = true;
        soundprite.raycastTarget = true;
        soundBtn.interactable = true;
        soundText.enabled = true;

        creditsSprite.enabled = true;
        creditsSprite.raycastTarget = true;
        creditsBtn.interactable = true;
        creditsText.enabled = true;

        quitSprite.enabled = true;
        quitSprite.raycastTarget = true;
        quitBtn.interactable = true;
        quitText.enabled = true;

        blanket.enabled = true;
        blanket.raycastTarget = true;
    }

    public void Hide()
    {
        pauseText.enabled = false;

        resumeSprite.enabled = false;
        resumeSprite.raycastTarget = false;
        resumeBtn.interactable = false;
        resumeText.enabled = false;

        howToPlaySprite.enabled = false;
        howToPlaySprite.raycastTarget = false;
        howToPlayBtn.interactable = false;
        howToPlayText.enabled = false;

        soundprite.enabled = false;
        soundprite.raycastTarget = false;
        soundBtn.interactable = false;
        soundText.enabled = false;

        creditsSprite.enabled = false;
        creditsSprite.raycastTarget = false;
        creditsBtn.interactable = false;
        creditsText.enabled = false;

        quitSprite.enabled = false;
        quitSprite.raycastTarget = false;
        quitBtn.interactable = false;
        quitText.enabled = false;

        blanket.enabled = false;
        blanket.raycastTarget = false;
    }

    private void ResumeClicked()
    {
        Hide();
        gameHUD.Show();
    }

    private void CreditsClick() {
        Hide();
        creditsScreen.Show();
    }

    private void SoundToggleClicked()
    {
        soundText.text = GameDataManager.Instance.ToggleSound() ? SOUND_ON_TEXT : SOUND_OFF_TEXT;
        SoundManager.Instance.StopAllSounds();
        SoundManager.Instance.PlayGameMusic(GameDataManager.Instance.IsSound);
    }

    private void HowToPlayClicked()
    {
        Hide();
        howToPlayScreen.Show();
    }

    private void QuitClicked()
    {
        Hide();
        mainMenuScreen.Show();
    }

    public bool isReady()
    {
        return ready;
    }
}
