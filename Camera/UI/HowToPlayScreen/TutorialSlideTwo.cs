using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideTwo : MonoBehaviour, UIScreen {
    private bool ready = false;

    private Image touchSplat;
    private Text touchSplatText;

    private Image swipeUpArrow;
    private Text swipeUpArrowGestureLabel;
    private Image swipeUpArrowActionPanel;
    private Text swipeUpArrowActionLabel;

    private Image swipeDownArrow;
    private Text swipeDownArrowGestureLabel;
    private Image swipeDownArrowActionPanel;
    private Text swipeDownArrowActionLabel;

    private Image swipeRightArrow;
    private Text swipeRightArrowGestureLabel;
    private Image swipeRightArrowActionPanel;
    private Text swipeRightArrowActionLabel;

    private Image swipeLeftArrow;
    private Text swipeLeftArrowGestureLabel;
    private Image swipeLeftArrowActionPanel;
    private Text swipeLeftArrowActionLabel;

    // Use this for initialization
    void Start () {
        GameObject tapOutline = transform.Search("Tap Outline").gameObject;
        touchSplat = tapOutline.GetComponent<Image>();
        touchSplatText = touchSplat.GetComponentInChildren<Text>();

        GameObject swipeUp = transform.Search("Swipe Up Arrow").gameObject;
        swipeUpArrow = swipeUp.GetComponent<Image>();
        swipeUpArrowGestureLabel = swipeUp.GetComponentInChildren<Text>();
        swipeUpArrowActionPanel = swipeUp.transform.Search("Action Panel").GetComponent<Image>();
        swipeUpArrowActionLabel = swipeUp.transform.Search("Action Panel").GetComponentInChildren<Text>();

        GameObject swipeDown = transform.Search("Swipe Down Arrow").gameObject;
        swipeDownArrow = swipeDown.GetComponent<Image>();
        swipeDownArrowGestureLabel = swipeDown.GetComponentInChildren<Text>();
        swipeDownArrowActionPanel = swipeDown.transform.Search("Action Panel").GetComponent<Image>();
        swipeDownArrowActionLabel = swipeDown.transform.Search("Action Panel").GetComponentInChildren<Text>();

        GameObject swipeRight = transform.Search("Swipe Right Arrow").gameObject;
        swipeRightArrow = swipeRight.GetComponent<Image>();
        swipeRightArrowGestureLabel = swipeRight.GetComponentInChildren<Text>();
        swipeRightArrowActionPanel = swipeRight.transform.Search("Action Panel").GetComponent<Image>();
        swipeRightArrowActionLabel = swipeRight.transform.Search("Action Panel").GetComponentInChildren<Text>();

        GameObject swipeLeft = transform.Search("Swipe Left Arrow").gameObject;
        swipeLeftArrow = swipeLeft.GetComponent<Image>();
        swipeLeftArrowGestureLabel = swipeLeft.GetComponentInChildren<Text>();
        swipeLeftArrowActionPanel = swipeLeft.transform.Search("Action Panel").GetComponent<Image>();
        swipeLeftArrowActionLabel = swipeLeft.transform.Search("Action Panel").GetComponentInChildren<Text>();

        ready = true;

        Hide();
    }

    public void Hide()
    {
        touchSplat.enabled = false;
        touchSplatText.enabled = false;

        swipeUpArrow.enabled = false;
        swipeUpArrowGestureLabel.enabled = false;
        swipeUpArrowActionPanel.enabled = false;
        swipeUpArrowActionLabel.enabled = false;

        swipeDownArrow.enabled = false;
        swipeDownArrowGestureLabel.enabled = false;
        swipeDownArrowActionPanel.enabled = false;
        swipeDownArrowActionLabel.enabled = false;

        swipeRightArrow.enabled = false;
        swipeRightArrowGestureLabel.enabled = false;
        swipeRightArrowActionPanel.enabled = false;
        swipeRightArrowActionLabel.enabled = false;

        swipeLeftArrow.enabled = false;
        swipeLeftArrowGestureLabel.enabled = false;
        swipeLeftArrowActionPanel.enabled = false;
        swipeLeftArrowActionLabel.enabled = false;
    }

    public void Show()
    {
        touchSplat.enabled = true;
        touchSplatText.enabled = true;

        swipeUpArrow.enabled = true;
        swipeUpArrowGestureLabel.enabled = true;
        swipeUpArrowActionPanel.enabled = true;
        swipeUpArrowActionLabel.enabled = true;

        swipeDownArrow.enabled = true;
        swipeDownArrowGestureLabel.enabled = true;
        swipeDownArrowActionPanel.enabled = true;
        swipeDownArrowActionLabel.enabled = true;

        swipeRightArrow.enabled = true;
        swipeRightArrowGestureLabel.enabled = true;
        swipeRightArrowActionPanel.enabled = true;
        swipeRightArrowActionLabel.enabled = true;

        swipeLeftArrow.enabled = true;
        swipeLeftArrowGestureLabel.enabled = true;
        swipeLeftArrowActionPanel.enabled = true;
        swipeLeftArrowActionLabel.enabled = true;
    }

    public bool isReady()
    {
        return ready;
    }
}
