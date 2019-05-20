using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    private GameHUD gameHUD;
    private GameOverScreen gameOverScreen;
    private HowToPlayScreen howToPlayScreen;

    public AudioSource AudioSource;

    public int maxNumOfEnemies = 1;
    private const int MAX_ENEMIES_EVER = 6;
    public int m_enemyCount = 0;
    private List<GameObject> m_enemies = new List<GameObject>();

    private ScoreCounter scoreCounter;

    private Player player;
    private readonly Vector3 PLAYER_SPAWN_POS = new Vector3(0, 40f, 0);

    private GameCamera gameCamera;

    private bool m_gamePaused = false;
    public bool GameSlowMo = false;

    private bool m_gameOver = false;

    void Awake()
    {
        GameDataManager.Instance.LoadGameData();
        ResourceManager.Instance.LoadResources();
        SoundManager.Instance.LoadSounds();
        AdManager.Instance.InitializeAds();
    }

    // Use this for initialization
    void Start()
    {
        gameHUD = GameObject.Find("HUD").GetComponent<GameHUD>();
        howToPlayScreen = gameHUD.GetComponentInChildren<HowToPlayScreen>();
        gameOverScreen = gameHUD.GetComponentInChildren<GameOverScreen>();

        gameCamera = Camera.main.GetComponent<GameCamera>();

        AudioSource = GetComponent<AudioSource>();

        SoundManager.Instance.PlayGameMusic(GameDataManager.Instance.IsSound);
    }

    void Update()
    {
        if (m_gamePaused)
        {
            GameSlowMo = false;
            Time.timeScale = 0;
        }
        if (GameSlowMo)
        {
            Time.timeScale = .05f;
        }
        if (!GameSlowMo && !m_gamePaused)
        {
            Time.timeScale = 1;
        }
    }

    public void StartNewGame()
    {
        CleanUpScene();

        if (GameDataManager.Instance.HighScore == 0)
        {
            m_gamePaused = true;
            howToPlayScreen.Show(false);
        }
        else {
            gameHUD.Show();
        }

        scoreCounter = GameObject.Find("Score").GetComponent<ScoreCounter>();
        scoreCounter.ResetScore();

        GameObject playerGO = Instantiate(ResourceManager.Instance.Player,
                PLAYER_SPAWN_POS,
                Actor.FACE_LEFT) as GameObject;
        player = playerGO.GetComponent<Player>();
        Camera.main.GetComponent<GameCamera>().Target = player.transform;

        InvokeRepeating("SpawnEnemy", 3f, 5f);

        SoundManager.Instance.BellRing(AudioSource);
    }

    public void GameOver()
    {
        if (!m_gameOver)
        {
            m_gameOver = true;
            GameSlowMo = false;
            SoundManager.Instance.BellRing(AudioSource);
            gameHUD.Hide();
            CancelInvoke();
            gameOverScreen.SetPlayerScore(scoreCounter.Points);
            if (scoreCounter.Points > GameDataManager.Instance.HighScore)
            {
                Instantiate(ResourceManager.Instance.HighScoreConfetti,
                    new Vector3(player.transform.position.x,
                                35,
                                0),
                    Quaternion.Euler(-90, 0, 0));
                GameDataManager.Instance.SetHighScore(scoreCounter.Points);
                SoundManager.Instance.CrowdCheer(AudioSource);
            }
            gameOverScreen.SetHighScore(GameDataManager.Instance.HighScore);
            gameOverScreen.Show();
        }
    }

    public void CleanUpScene()
    {
        GameSlowMo = false;
        gameHUD.Hide();
        for (int i = 0; i < m_enemies.Count; i++)
        {
            Destroy(m_enemies[i].gameObject);
        }
        m_enemies.Clear();
        m_enemyCount = 0;
        maxNumOfEnemies = 1;
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        gameOverScreen.Hide();
        m_gameOver = false;
        m_gamePaused = false;
        CancelInvoke();
    }

    void SpawnEnemy()
    {
        if (m_enemyCount < maxNumOfEnemies && m_enemyCount < MAX_ENEMIES_EVER)
        {
            // Determine Spawn Point
            float distToRight = Vector3.Distance(player.transform.position, Ring.RIGHT_ROPES_POS);
            float distToLeft = Vector3.Distance(player.transform.position, Ring.LEFT_ROPES_POS);
            Vector3 modLeftRopePos = new Vector3(
                    Ring.LEFT_ROPES_POS.x - 5,
                    Ring.LEFT_ROPES_POS.y,
                    Ring.LEFT_ROPES_POS.z
                );
            Vector3 modRightRopePos = new Vector3(
                    Ring.RIGHT_ROPES_POS.x + 5,
                    Ring.RIGHT_ROPES_POS.y,
                    Ring.RIGHT_ROPES_POS.z
                );

            Vector2 spawnPos = distToRight < distToLeft ? modLeftRopePos : modRightRopePos;

            // Spawn Enemy
            GameObject enemy = Instantiate(ResourceManager.Instance.Enemy,
                spawnPos,
                Quaternion.identity) as GameObject;

            // Add enemy to enemy array for tracking
            AddEnemyToEnemyList(enemy);
        }
    }

    private void AddEnemyToEnemyList(GameObject enemy)
    {
        m_enemies.Add(enemy);
        m_enemyCount = m_enemies.Count;
    }

    private void RemoveEnemyFromEnemyList(EnemyWrestler enemy)
    {
        for (int i = 0; i < m_enemies.Count; i++)
        {
            EnemyWrestler wrestler = m_enemies[i].GetComponent<EnemyWrestler>();
            if (wrestler.ID.Equals(enemy.ID))
            {
                if (!m_gameOver)
                {
                    scoreCounter.IncrementScore();
                    SoundManager.Instance.CrowdCheer(AudioSource);
                }
                m_enemies.RemoveAt(i);
                Destroy(enemy.gameObject);
                m_enemyCount = m_enemies.Count;

                int newMaxEnemies = Mathf.RoundToInt(Score / 1.25f);
                maxNumOfEnemies = newMaxEnemies >= 1 ? newMaxEnemies : 1;
                return;
            }
        }
    }

    public void DestroyEnemy(EnemyWrestler enemy)
    {
        RemoveEnemyFromEnemyList(enemy);
    }

    public IEnumerator DynamicDamageView(Vector2 force, Actor attacker, Actor reciever)
    {
        if (Mathf.Abs(force.x) >= 375)
        {
            float transitionDuration = .05f;
            gameHUD.Hide();
            StartCoroutine(gameCamera.ZoomToCinematicActors(transitionDuration, attacker, reciever));
            SoundManager.Instance.FadeOutAllSounds(transitionDuration);
            yield return new WaitForSeconds(transitionDuration);
            SoundManager.Instance.SingleBellRing(AudioSource);
            Vector3 spawnPos = Vector3.Lerp(attacker.transform.position, reciever.transform.position, .5f);
            GameObject dynamicDmgBackground = Instantiate(
                ResourceManager.Instance.DynamicDamage,
                new Vector3(spawnPos.x, spawnPos.y, spawnPos.z - 5),
                Quaternion.identity
                ) as GameObject;
            dynamicDmgBackground.transform.LookAt(gameCamera.transform);
            GameSlowMo = true;
            Destroy(dynamicDmgBackground, .1f);
            Invoke("FinishDynamicDamageView", .1f);
        }
    }

    private void FinishDynamicDamageView()
    {
        SoundManager.Instance.PlayGameMusic(true);
        GameSlowMo = false;
        gameCamera.ResetCameraToGameView();
        gameHUD.Show();
    }

    public List<GameObject> Enemies
    {
        get { return m_enemies; }
    }

    public int Score
    {
        get { return scoreCounter.Points; }
    }

    public bool isPaused
    {
        get { return m_gamePaused; }
    }

    public void Pause()
    {
        m_gamePaused = true;
    }

    public void Resume()
    {
        m_gamePaused = false;
    }
}
