using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideOne : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private Image playerArrow;
    private Text playerArrowLabel;

    private Image moveArrow;
    private Text moveArrowLabel;

    private Image pauseArrow;
    private Text pauseArrowLabel;

    private Image scoreArrow;
    private Text scoreArrowLabel;

    // Use this for initialization
    void Start()
    {
        GameObject playerGameObject = transform.Search("Player Arrow").gameObject;
        playerArrow = playerGameObject.GetComponent<Image>();
        playerArrowLabel = playerGameObject.GetComponentInChildren<Text>();
        GameObject moveGameObject = transform.Search("Move Arrow").gameObject;
        moveArrow = moveGameObject.GetComponent<Image>();
        moveArrowLabel = moveGameObject.GetComponentInChildren<Text>();
        GameObject pauseGameObject = transform.Search("Pause Arrow").gameObject;
        pauseArrow = pauseGameObject.GetComponent<Image>();
        pauseArrowLabel = pauseGameObject.GetComponentInChildren<Text>();
        GameObject scoreGameObject = transform.Search("Score Arrow").gameObject;
        scoreArrow = scoreGameObject.GetComponent<Image>();
        scoreArrowLabel = scoreGameObject.GetComponentInChildren<Text>();

        ready = true;

        Hide();
    }

    public void Hide()
    {
        playerArrow.enabled = false;
        playerArrowLabel.enabled = false;
        moveArrow.enabled = false;
        moveArrowLabel.enabled = false;
        pauseArrow.enabled = false;
        pauseArrowLabel.enabled = false;
        scoreArrow.enabled = false;
        scoreArrowLabel.enabled = false;
    }

    public void Show()
    {
        playerArrow.enabled = true;
        playerArrowLabel.enabled = true;
        moveArrow.enabled = true;
        moveArrowLabel.enabled = true;
        pauseArrow.enabled = true;
        pauseArrowLabel.enabled = true;
        scoreArrow.enabled = true;
        scoreArrowLabel.enabled = true;
    }

    public bool isReady()
    {
        return ready;
    }
}
