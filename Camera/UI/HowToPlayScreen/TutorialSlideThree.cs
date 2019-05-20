using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideThree : MonoBehaviour, UIScreen
{
    private bool ready;

    private Image touchSplat;
    private Text touchSplatText;

    private Image swipeUpArrow;
    private Text swipeUpArrowGestureLabel;

    private Image swipeDownArrow;
    private Text swipeDownArrowGestureLabel;
    private Image swipeDownArrowActionPanel;
    private Text swipeDownArrowActionLabel;

    private Image swipeDiagRightArrow;
    private Text swipeDiagRightArrowGestureLabel;

    private Image swipeDiagLeftArrow;
    private Text swipeDiagLeftArrowGestureLabel;

    private Text instructions1;
    private Text instructions2;
    private Text instructions3;

    // Use this for initialization
    void Start()
    {
        GameObject tapOutline = transform.Search("Tap Outline").gameObject;
        touchSplat = tapOutline.GetComponent<Image>();
        touchSplatText = touchSplat.GetComponentInChildren<Text>();

        GameObject swipeUp = transform.Search("Swipe Up Arrow").gameObject;
        swipeUpArrow = swipeUp.GetComponent<Image>();
        swipeUpArrowGestureLabel = swipeUp.GetComponentInChildren<Text>();

        GameObject swipeDown = transform.Search("Swipe Down Arrow").gameObject;
        swipeDownArrow = swipeDown.GetComponent<Image>();
        swipeDownArrowGestureLabel = swipeDown.GetComponentInChildren<Text>();
        swipeDownArrowActionPanel = swipeDown.transform.Search("Action Panel").GetComponent<Image>();
        swipeDownArrowActionLabel = swipeDown.transform.Search("Action Panel").GetComponentInChildren<Text>();

        GameObject swipeRight = transform.Search("Swipe Diag Right Arrow").gameObject;
        swipeDiagRightArrow = swipeRight.GetComponent<Image>();
        swipeDiagRightArrowGestureLabel = swipeRight.GetComponentInChildren<Text>();

        GameObject swipeLeft = transform.Search("Swipe Diag Left Arrow").gameObject;
        swipeDiagLeftArrow = swipeLeft.GetComponent<Image>();
        swipeDiagLeftArrowGestureLabel = swipeLeft.GetComponentInChildren<Text>();

        instructions1 = transform.Search("Instructions Pt 1").GetComponent<Text>();
        instructions2 = transform.Search("Instructions Pt 2").GetComponent<Text>();
        instructions3 = transform.Search("Instructions Pt 3").GetComponent<Text>();

        ready = true;

        Hide();
    }

    public void Hide()
    {
        touchSplat.enabled = false;
        touchSplatText.enabled = false;

        swipeUpArrow.enabled = false;
        swipeUpArrowGestureLabel.enabled = false;

        swipeDownArrow.enabled = false;
        swipeDownArrowGestureLabel.enabled = false;
        swipeDownArrowActionPanel.enabled = false;
        swipeDownArrowActionLabel.enabled = false;

        swipeDiagRightArrow.enabled = false;
        swipeDiagRightArrowGestureLabel.enabled = false;

        swipeDiagLeftArrow.enabled = false;
        swipeDiagLeftArrowGestureLabel.enabled = false;

        instructions1.enabled = false;
        instructions2.enabled = false;
        instructions3.enabled = false;
    }

    public void Show()
    {
        touchSplat.enabled = true;
        touchSplatText.enabled = true;

        swipeUpArrow.enabled = true;
        swipeUpArrowGestureLabel.enabled = true;

        swipeDownArrow.enabled = true;
        swipeDownArrowGestureLabel.enabled = true;
        swipeDownArrowActionPanel.enabled = true;
        swipeDownArrowActionLabel.enabled = true;

        swipeDiagRightArrow.enabled = true;
        swipeDiagRightArrowGestureLabel.enabled = true;

        swipeDiagLeftArrow.enabled = true;
        swipeDiagLeftArrowGestureLabel.enabled = true;

        instructions1.enabled = true;
        instructions2.enabled = true;
        instructions3.enabled = true;
    }

    public bool isReady()
    {
        return ready;
    }
}
