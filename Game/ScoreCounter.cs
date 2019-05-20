using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreCounter : MonoBehaviour
{
    private Text m_pointText;
    private int m_points = 0;

    // Use this for initialization
    void Start()
    {
        m_pointText = GetComponent<Text>();
        m_pointText.text = m_points.ToString();
    }

    public void IncrementScore()
    {
        if (m_pointText != null)
        {
            m_points++;
            m_pointText.text = m_points.ToString();
        }
    }

    public int Points
    {
        get { return m_points; }
    }

    public void ResetScore()
    {
        if (m_pointText != null)
        {
            m_points = 0;
            m_pointText.text = m_points.ToString();
        }
    }
}
