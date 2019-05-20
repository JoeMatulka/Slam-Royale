using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private bool isAdShowing = false;

    private GameManager gameManager;

    private Text gameOverText;

    private Image scorePanel;

    private Text loadingText;

    private Text highScoreLabel;
    private Text highScoreText;
    private int highScoreValue;

    private Text playerScoreLabel;
    private Text playerScoreText;
    private int playerScoreValue;

    private Button tryAgainBtn;
    private Image tryAgainBtnSprite;
    private Text tryAgainText;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        gameOverText = transform.Search("Game Over Text").gameObject.GetComponent<Text>();

        scorePanel = transform.Search("Score Panel").gameObject.GetComponent<Image>();

        loadingText = transform.Search("Loading Text").gameObject.GetComponent<Text>();

        highScoreLabel = transform.Search("High Score Label").gameObject.GetComponent<Text>();
        highScoreText = highScoreLabel.transform.Search("High Score Value").gameObject.GetComponent<Text>();

        playerScoreLabel = transform.Search("Player Score Label").gameObject.GetComponent<Text>();
        playerScoreText = playerScoreLabel.transform.Search("Player Score Value").gameObject.GetComponent<Text>();

        tryAgainBtn = transform.Search("Try Again Button").gameObject.GetComponent<Button>();
        tryAgainBtn.onClick.AddListener(TryAgainClick);
        tryAgainBtnSprite = transform.Search("Try Again Button").gameObject.GetComponent<Image>();
        tryAgainText = tryAgainBtn.transform.Search("Text").gameObject.GetComponent<Text>();

        ready = true;

        Hide();
    }

    private void SetUpAdListeners()
    {
        // Called when an ad request failed to load.
        AdManager.Instance.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when the ad is closed.
        AdManager.Instance.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        AdManager.Instance.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        isAdShowing = false;
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        isAdShowing = false;
        AdManager.Instance.interstitial.Destroy();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Error from: " + sender);
        Debug.Log("Interstitial failed to load: " + args.Message);
        AdManager.Instance.errorLoadingAd = true;
        isAdShowing = false;
    }

    private void TryAgainClick()
    {
        StartCoroutine(StartNewGame());
    }

    public IEnumerator StartNewGame() {
        if (AdManager.Instance.CanShowAd())
        {
            Hide();

            isAdShowing = true;

            SoundManager.Instance.StopAllSounds();

            loadingText.enabled = true;

            scorePanel.raycastTarget = true;
            scorePanel.enabled = true;

            StartCoroutine(AdManager.Instance.ShowInterstitialAd());
            yield return new WaitUntil(() => !isAdShowing);
            SoundManager.Instance.PlayGameMusic(GameDataManager.Instance.IsSound);
        }
        gameManager.StartNewGame();
    }

    public void SetPlayerScore(int score)
    {
        playerScoreValue = score;
        playerScoreText.text = playerScoreValue.ToString();
    }

    public void SetHighScore(int score)
    {
        highScoreValue = score;
        highScoreText.text = highScoreValue.ToString();
    }

    public void Hide()
    {
        gameOverText.enabled = false;

        loadingText.enabled = false;

        scorePanel.raycastTarget = false;
        scorePanel.enabled = false;

        highScoreLabel.enabled = false;
        highScoreText.enabled = false;

        playerScoreLabel.enabled = false;
        playerScoreText.enabled = false;

        tryAgainBtn.interactable = false;
        tryAgainBtnSprite.raycastTarget = false;
        tryAgainBtnSprite.enabled = false;
        tryAgainText.enabled = false;
    }

    public void Show()
    {
        AdManager.Instance.RequestInterstitialAd();
        SetUpAdListeners();
        gameOverText.enabled = true;

        scorePanel.raycastTarget = true;
        scorePanel.enabled = true;

        highScoreLabel.enabled = true;
        highScoreText.enabled = true;

        playerScoreLabel.enabled = true;
        playerScoreText.enabled = true;

        tryAgainBtn.interactable = true;
        tryAgainBtnSprite.raycastTarget = true;
        tryAgainBtnSprite.enabled = true;
        tryAgainText.enabled = true;
    }

    public bool isReady()
    {
        return ready;
    }
}
