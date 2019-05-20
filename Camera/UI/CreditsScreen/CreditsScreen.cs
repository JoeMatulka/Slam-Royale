using UnityEngine;
using UnityEngine.UI;

public class CreditsScreen : MonoBehaviour, UIScreen
{
    private bool m_isReady = false;

    private PauseScreen pauseScreen;

    private Text creditsTitle;
    private Text credits;
    private Image creditsBlanket;

    private Button closeBtn;
    private Image closeSprite;

    // Use this for initialization
    void Start()
    {
        pauseScreen = GameObject.Find("Pause Screen").GetComponent<PauseScreen>();

        creditsTitle = transform.Search("Credits Title").GetComponent<Text>();
        credits = transform.Search("Credits").GetComponent<Text>();
        creditsBlanket = transform.Search("Credits Blanket").GetComponent<Image>();

        GameObject close = transform.Search("Close Button").gameObject;
        closeBtn = close.GetComponent<Button>();
        closeBtn.onClick.AddListener(Exit);
        closeSprite = close.GetComponent<Image>();

        Hide();
        m_isReady = true;
    }

    public void Hide()
    {
        creditsTitle.enabled = false;
        credits.enabled = false;
        creditsBlanket.enabled = false;
        creditsBlanket.raycastTarget = false;

        closeSprite.enabled = false;
        closeSprite.raycastTarget = false;
        closeBtn.interactable = false;
    }

    public bool isReady()
    {
        return m_isReady;
    }

    public void Show()
    {
        creditsTitle.enabled = true;
        credits.enabled = true;
        creditsBlanket.enabled = true;
        creditsBlanket.raycastTarget = true;

        closeSprite.enabled = true;
        closeSprite.raycastTarget = true;
        closeBtn.interactable = true;
    }

    public void Exit()
    {
        Hide();
        pauseScreen.Show();
    }
}
