using UnityEngine;
using UnityEngine.UI;

public class TutorialSlideFour : MonoBehaviour, UIScreen
{
    private bool ready = false;

    private Text instructions1;
    private Text instructions2;
    private Text instructions3;

    // Use this for initialization
    void Start()
    {
        instructions1 = transform.Search("Instructions Pt 1").GetComponent<Text>();
        instructions2 = transform.Search("Instructions Pt 2").GetComponent<Text>();
        instructions3 = transform.Search("Instructions Pt 3").GetComponent<Text>();

        ready = true;

        Hide();
    }

    public void Hide()
    {
        instructions1.enabled = false;
        instructions2.enabled = false;
        instructions3.enabled = false;
    }

    public void Show()
    {
        instructions1.enabled = true;
        instructions2.enabled = true;
        instructions3.enabled = true;
    }

    public bool isReady()
    {
        return ready;
    }
}
