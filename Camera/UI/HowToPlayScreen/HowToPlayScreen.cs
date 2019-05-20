using UnityEngine;
using UnityEngine.UI;

public class HowToPlayScreen : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private GameHUD gameHUD;
    private bool resumeGameOnClose = false;

    private Sprite[] slides;
    private int currentSlideIndex = 0;

    private TutorialSlideOne tutorialSlideOneGraphics;
    private TutorialSlideTwo tutorialSlideTwoGraphics;
    private TutorialSlideThree tutorialSlideThreeGraphics;
    private TutorialSlideFour tutorialSlideFourGraphics;

    private PauseScreen pauseScreen;

    private Text howToPlayText;

    private Image blanket;

    private Image displaySlide;

    private Button nextSlideBtn;
    private Image nextSlideSprite;

    private Button previousSlideBtn;
    private Image previousSlideSprite;

    private Button closeBtn;
    private Image closeSprite;

    private Image slideBubble1;
    private Image slideBubble2;
    private Image slideBubble3;
    private Image slideBubble4;

    // Use this for initialization
    void Start()
    {
        gameHUD = GetComponentInParent<GameHUD>();

        slides = new Sprite[4] {
                ResourceManager.Instance.TutorialImageControls,
                ResourceManager.Instance.TutorialImageNoControls,
                ResourceManager.Instance.TutorialImageGrabbing,
                ResourceManager.Instance.TutorialImageHowToScore,
        };

        tutorialSlideOneGraphics = GetComponentInChildren<TutorialSlideOne>();
        tutorialSlideTwoGraphics = GetComponentInChildren<TutorialSlideTwo>();
        tutorialSlideThreeGraphics = GetComponentInChildren<TutorialSlideThree>();
        tutorialSlideFourGraphics = GetComponentInChildren<TutorialSlideFour>();

        pauseScreen = GameObject.Find("Pause Screen").GetComponent<PauseScreen>();

        howToPlayText = transform.Search("How To Play Text").GetComponent<Text>();

        blanket = transform.Search("Blanket").GetComponent<Image>();

        displaySlide = transform.Search("Display Slide").GetComponent<Image>();

        GameObject nextSlide = transform.Search("Next Slide Button").gameObject;
        nextSlideBtn = nextSlide.GetComponent<Button>();
        nextSlideBtn.onClick.AddListener(NextSlide);
        nextSlideSprite = nextSlide.GetComponent<Image>();

        GameObject previousSlide = transform.Search("Previous Slide Button").gameObject;
        previousSlideBtn = previousSlide.GetComponent<Button>();
        previousSlideBtn.onClick.AddListener(PreviousSlide);
        previousSlideSprite = previousSlide.GetComponent<Image>();

        GameObject close = transform.Search("Close Button").gameObject;
        closeBtn = close.GetComponent<Button>();
        closeBtn.onClick.AddListener(Exit);
        closeSprite = close.GetComponent<Image>();

        slideBubble1 = transform.Search("Slide 1 Bubble").GetComponent<Image>();
        slideBubble2 = transform.Search("Slide 2 Bubble").GetComponent<Image>();
        slideBubble3 = transform.Search("Slide 3 Bubble").GetComponent<Image>();
        slideBubble4 = transform.Search("Slide 4 Bubble").GetComponent<Image>();

        ready = true;

        Hide();
    }

    private void NextSlide()
    {
        if (currentSlideIndex + 1 <= slides.Length - 1)
        {
            currentSlideIndex++;
            displaySlide.sprite = slides[currentSlideIndex];
            UpdateDisplayGraphicsBySlideIndex();
        }
    }

    private void UpdateDisplayGraphicsBySlideIndex()
    {
        switch (currentSlideIndex)
        {
            case 0:
                howToPlayText.text = "HOW TO PLAY - CONTROLS";
                slideBubble1.sprite = ResourceManager.Instance.FilledCircle;
                slideBubble2.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble3.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble4.sprite = ResourceManager.Instance.EmptyCircle;
                tutorialSlideOneGraphics.Show();
                tutorialSlideTwoGraphics.Hide();
                tutorialSlideThreeGraphics.Hide();
                tutorialSlideFourGraphics.Hide();
                break;
            case 1:
                howToPlayText.text = "HOW TO PLAY - ATTACKING";
                slideBubble1.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble2.sprite = ResourceManager.Instance.FilledCircle;
                slideBubble3.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble4.sprite = ResourceManager.Instance.EmptyCircle;
                tutorialSlideOneGraphics.Hide();
                tutorialSlideTwoGraphics.Show();
                tutorialSlideThreeGraphics.Hide();
                tutorialSlideFourGraphics.Hide();
                break;
            case 2:
                howToPlayText.text = "HOW TO PLAY - GRABBING";
                slideBubble1.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble2.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble3.sprite = ResourceManager.Instance.FilledCircle;
                slideBubble4.sprite = ResourceManager.Instance.EmptyCircle;
                tutorialSlideOneGraphics.Hide();
                tutorialSlideTwoGraphics.Hide();
                tutorialSlideThreeGraphics.Show();
                tutorialSlideFourGraphics.Hide();
                break;
            case 3:
                howToPlayText.text = "HOW TO PLAY - SCORING";
                slideBubble1.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble2.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble3.sprite = ResourceManager.Instance.EmptyCircle;
                slideBubble4.sprite = ResourceManager.Instance.FilledCircle;
                tutorialSlideOneGraphics.Hide();
                tutorialSlideTwoGraphics.Hide();
                tutorialSlideThreeGraphics.Hide();
                tutorialSlideFourGraphics.Show();
                break;
        }
    }

    private void PreviousSlide()
    {
        if (currentSlideIndex - 1 >= 0)
        {
            currentSlideIndex--;
            displaySlide.sprite = slides[currentSlideIndex];
            UpdateDisplayGraphicsBySlideIndex();
        }
    }

    private void Exit()
    {
        Hide();
        if (!resumeGameOnClose)
        {
            pauseScreen.Show();
        }
        else {
            gameHUD.Show();
        }
    }

    public void Hide()
    {
        howToPlayText.enabled = false;

        blanket.enabled = false;
        blanket.raycastTarget = false;

        displaySlide.enabled = false;
        displaySlide.raycastTarget = false;

        nextSlideSprite.enabled = false;
        nextSlideSprite.raycastTarget = false;
        nextSlideBtn.interactable = false;

        previousSlideSprite.enabled = false;
        previousSlideSprite.raycastTarget = false;
        previousSlideBtn.interactable = false;

        closeSprite.enabled = false;
        closeSprite.raycastTarget = false;
        closeBtn.interactable = false;

        slideBubble1.enabled = false;
        slideBubble2.enabled = false;
        slideBubble3.enabled = false;
        slideBubble4.enabled = false;

        if (tutorialSlideOneGraphics.isReady())
        {
            tutorialSlideOneGraphics.Hide();
        }
        if (tutorialSlideTwoGraphics.isReady())
        {
            tutorialSlideTwoGraphics.Hide();
        }
        if (tutorialSlideThreeGraphics.isReady())
        {
            tutorialSlideThreeGraphics.Hide();
        }
        if (tutorialSlideFourGraphics.isReady())
        {
            tutorialSlideFourGraphics.Hide();
        }
    }

    public void Show()
    {
        Show(true);
    }

    public void Show(bool fromPause)
    {
        resumeGameOnClose = !fromPause;

        currentSlideIndex = 0;
        displaySlide.sprite = slides[currentSlideIndex];
        displaySlide.enabled = true;
        displaySlide.raycastTarget = true;

        howToPlayText.enabled = true;

        blanket.enabled = true;
        blanket.raycastTarget = true;

        nextSlideSprite.enabled = true;
        nextSlideSprite.raycastTarget = true;
        nextSlideBtn.interactable = true;

        previousSlideSprite.enabled = true;
        previousSlideSprite.raycastTarget = true;
        previousSlideBtn.interactable = true;

        closeSprite.enabled = true;
        closeSprite.raycastTarget = true;
        closeBtn.interactable = true;

        slideBubble1.enabled = true;
        slideBubble2.enabled = true;
        slideBubble3.enabled = true;
        slideBubble4.enabled = true;
        UpdateDisplayGraphicsBySlideIndex();
    }

    public bool isReady()
    {
        return ready;
    }
}
