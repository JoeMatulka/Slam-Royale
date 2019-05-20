using GoogleMobileAds.Api;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    private const string HIGH_SCORE_KEY = "high_score";

    private const string SOUND_TOGGLE_KEY = "sound_toggle";
    private const string SOUND_ON_KEY = "on";
    private const string SOUND_OFF_KEY = "off";

    private const string FIRST_TIME_KEY = "first_time_playing";

    private int m_highScore;

    private bool m_isSound;

    protected GameDataManager() { }

    public void LoadGameData()
    {
        m_highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY);
        m_isSound = PlayerPrefs.GetString(SOUND_TOGGLE_KEY).Equals(SOUND_ON_KEY);
    }

    public bool ToggleSound()
    {
        m_isSound = !m_isSound;
        PlayerPrefs.SetString(SOUND_TOGGLE_KEY, m_isSound ? SOUND_ON_KEY : SOUND_OFF_KEY);
        PlayerPrefs.Save();
        return m_isSound;
    }

    public void SetHighScore(int score)
    {
        m_highScore = score;
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, m_highScore);
        PlayerPrefs.Save();
    }

    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public int HighScore
    {
        get { return m_highScore; }
    }

    public bool IsSound
    {
        get { return m_isSound; }
    }
}
