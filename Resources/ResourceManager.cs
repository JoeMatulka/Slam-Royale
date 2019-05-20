using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    // Actors
    private Object m_player;
    private Object m_enemy;

    // Effects
    private Object m_splashDamage;
    private Object m_hitSpark;
    private Object m_enemyGroundContact;
    private Material m_enemyGrndContactMat;
    private Object m_playerGroundContact;
    private Material m_playerGrndContactMat;
    private Object m_confetti;
    private Object m_highScoreConfetti;

    private Object m_dynamicDmg;

    // UI
    private Sprite m_tutorialControls;
    private Sprite m_tutorialNoControls;
    private Sprite m_tutorialGrab;
    private Sprite m_tutorialHowToScore;

    private Sprite m_emptyCircle;
    private Sprite m_filledCircle;

    // (Optional) Prevent non-singleton constructor use.
    protected ResourceManager() { }

    public void LoadResources()
    {
        // Actors
        m_player = Resources.Load("Player/Player");
        m_enemy = Resources.Load("Enemy/Enemy Wrestler");

        // Effects
        m_splashDamage = Resources.Load("Attack/Splash Damage");
        m_hitSpark = Resources.Load("Effects/Contacts/Hit");
        m_enemyGroundContact = Resources.Load("Effects/Contacts/Enemy Ground Contact");
        m_enemyGrndContactMat = (Material)Resources.Load("Effects/Contacts/EnemyGroundContact", typeof(Material));
        m_playerGroundContact = Resources.Load("Effects/Contacts/Player Ground Contact");
        m_playerGrndContactMat = (Material)Resources.Load("Effects/Contacts/PlayerGroundContact", typeof(Material));
        m_confetti = Resources.Load("Effects/Confetti");
        m_highScoreConfetti = Resources.Load("Effects/HighScoreConfetti");

        m_dynamicDmg = Resources.Load("Effects/Contacts/Dynamic Damage");

        // UI
        m_tutorialControls = (Sprite)Resources.Load("UI/Tutorial/tutorial-controls", typeof(Sprite));
        m_tutorialNoControls = (Sprite)Resources.Load("UI/Tutorial/tutorial-no-control", typeof(Sprite));
        m_tutorialGrab = (Sprite)Resources.Load("UI/Tutorial/tutorial-screen-grabbing", typeof(Sprite));
        m_tutorialHowToScore = (Sprite)Resources.Load("UI/Tutorial/tutorial-how-to-score", typeof(Sprite));

        m_emptyCircle = (Sprite)Resources.Load("UI/Tutorial/empty_circle", typeof(Sprite));
        m_filledCircle = (Sprite)Resources.Load("UI/Tutorial/filled_circle", typeof(Sprite));
    }

    public Object Player
    {
        get
        {
            return m_player;
        }
    }

    public Object Enemy
    {
        get
        {
            return m_enemy;
        }
    }

    public Object SplashDamage
    {
        get
        {
            return m_splashDamage;
        }
    }

    public Object DynamicDamage
    {
        get
        {
            return m_dynamicDmg;
        }
    }

    public Object HitSpark
    {
        get
        {
            return m_hitSpark;
        }
    }

    public Object Confetti
    {
        get
        {
            return m_confetti;
        }
    }

    public Object HighScoreConfetti
    {
        get
        {
            return m_highScoreConfetti;
        }
    }

    public Object EnemyGroundContact
    {
        get
        {
            return m_enemyGroundContact;
        }
    }

    public Material EnemyGroundContactMat
    {
        get
        {
            return m_enemyGrndContactMat;
        }
    }

    public Object PlayerGroundContact
    {
        get
        {
            return m_playerGroundContact;
        }
    }

    public Material PlayerGroundContactMat
    {
        get
        {
            return m_playerGrndContactMat;
        }
    }

    public Sprite TutorialImageControls
    {
        get
        {
            return m_tutorialControls;
        }
    }

    public Sprite TutorialImageNoControls
    {
        get
        {
            return m_tutorialNoControls;
        }
    }

    public Sprite TutorialImageGrabbing
    {
        get
        {
            return m_tutorialGrab;
        }
    }

    public Sprite TutorialImageHowToScore
    {
        get
        {
            return m_tutorialHowToScore;
        }
    }

    public Sprite EmptyCircle
    {
        get
        {
            return m_emptyCircle;
        }
    }

    public Sprite FilledCircle
    {
        get
        {
            return m_filledCircle;
        }
    }
}
