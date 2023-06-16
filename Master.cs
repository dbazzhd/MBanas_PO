using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour {
    private static Master instance = null;

    [SerializeField] private GameObject m_panel_mainMenu = null;
    [SerializeField] private GameObject m_panel_settings = null;
    [SerializeField] private GameObject m_panel_inGame = null;
    [SerializeField] private GameObject m_button_startGame = null;
    [SerializeField] private Slider m_slider_power = null;
    [SerializeField] private TMPro.TMP_Text m_text_power = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerOne = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerTwo = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerOneBalls = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerTwoBalls = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerOneKind = null;
    [SerializeField] private TMPro.TMP_Text m_text_playerTwoKind = null;
    [SerializeField] private GameObject m_panel_power = null;
    [SerializeField] private GameObject m_panel_foul = null;
    [SerializeField] private GameObject m_panel_win = null;
    [SerializeField] private GameObject m_panel_loss = null;

    [SerializeField] private PhysicMaterial m_physics_ball = null;
    [SerializeField] private PhysicMaterial m_physics_table = null;

    [SerializeField] private GameObject m_prefab_ball = null;
    [SerializeField] private GameObject m_prefab_cue = null;

    private Queue<Action<Master>> m_queueEvents = null;

    private bool m_gameStartedOnce = false;

    private List<BallBehavior> m_ballBehavior_balls = null;
    private BallBehavior m_ballBehavior_whiteBall = null;
    private CueBehavior m_cueBehavior = null;
    private CameraBehavior m_cameraBehavior = null;

    [SerializeField] private float m_cueRotateSpeed = 30.0f;
    [SerializeField] private float m_maxPower = 1000.0f;
    public float Power => instance.m_maxPower * instance.m_slider_power.value / 100.0f;
    private Color Orange = new Color(1.0f, 0.63f, 0.0f, 1.0f);


    private Player m_playerOne = null;
    private Player m_playerTwo = null;
    private Player m_currentPlayer = null;
    private Player m_otherPlayer = null;

    [SerializeField] private float m_minVelocity = 0.01f;
    private bool m_checkMovement = false;
    private bool m_otherKindPotted = false;
    private bool m_ballPotted = false;
    private bool m_whiteBallPotted = false;
    private bool m_blackBallPotted = false;

    void Start() {
        instance = this;

        m_queueEvents = new Queue<Action<Master>>();
        m_gameStartedOnce = false;
        m_ballBehavior_balls = new List<BallBehavior>();

        m_panel_mainMenu.SetActive(true);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnPress_ESC();
        }

        if (m_checkMovement) {
            CheckMovement();
        }
    }

    void LateUpdate() {
        ProcessEvents();
    }

    private void CheckMovement() {
        bool moving = false;
        foreach (BallBehavior behaviorBall in m_ballBehavior_balls) {
            if (!behaviorBall.IsSleeping()) {
                moving = true;
                break;
            }
        }
        if (!moving) {
            if (!m_ballBehavior_whiteBall.IsSleeping()) {
                moving = true;
            }
            if (!moving) {
                foreach (BallBehavior behaviorBall in m_ballBehavior_balls) {
                    behaviorBall.StopVelocity();
                }
                m_ballBehavior_whiteBall.StopVelocity();
                NextRound();
            }
        }
    }

    private void Initialize_PhysicsMaterials() {
        m_physics_ball.dynamicFriction = 0.25f;
        m_physics_ball.staticFriction = 0.25f;
        m_physics_ball.bounciness = 0.25f;
        m_physics_ball.frictionCombine = PhysicMaterialCombine.Multiply;
        m_physics_ball.bounceCombine = PhysicMaterialCombine.Average;

        m_physics_table.dynamicFriction = 0.25f;
        m_physics_table.staticFriction = 0.25f;
        m_physics_table.bounciness = 0.25f;
        m_physics_table.frictionCombine = PhysicMaterialCombine.Multiply;
        m_physics_table.bounceCombine = PhysicMaterialCombine.Average;
    }
    private void Initialize_Objects() {
        #region Colored Balls
        // Colored Balls
        float x = m_prefab_ball.GetComponent<SphereCollider>().radius;
        float z = 0.0475f;
        float d = x * 2.0f;

        CreateBall(0f, z * 2f, new Color(1f, 0.729f, 0f), 1, "Ball");            // Orange

        CreateBall(-x, z, new Color(0.631f, 0.439f, 0.078f), 0, "Ball");        // Brown
        CreateBall(x, z, new Color(0.031f, 0.722f, 0.435f), 0, "Ball");         // Green

        CreateBall(-d, 0f, new Color(0.671f, 0f, 1f), 1, "Ball");               // Purple
        CreateBall(0f, 0f, Color.black, 1, "Black");                             // Black
        CreateBall(d, 0f, new Color(1f, 0.663f, 0.953f), 1, "Ball");            // Pink

        CreateBall(-d - x, -z, new Color(1f, 0f, 0f), 1, "Ball");               // Red
        CreateBall(-x, -z, new Color(1f, 0.729f, 0f), 0, "Ball");               // Orange
        CreateBall(x, -z, new Color(0.631f, 0.439f, 0.078f), 1, "Ball");        // Brown
        CreateBall(d + x, -z, new Color(0.671f, 0f, 1f), 0, "Ball");            // Purple

        CreateBall(-d * 2f, -z * 2f, new Color(0f, 0f, 1f), 0, "Ball");         // Blue
        CreateBall(-d, -z * 2f, new Color(0.031f, 0.722f, 0.435f), 1, "Ball");  // Green
        CreateBall(0f, -z * 2f, new Color(1f, 0.663f, 0.953f), 0, "Ball");      // Pink
        CreateBall(d, -z * 2f, new Color(1f, 0f, 0f), 0, "Ball");               // Red
        CreateBall(d * 2f, -z * 2f, new Color(0f, 0f, 1f), 1, "Ball");          // Blue
        #endregion

        // White Ball
        GameObject gameObjectBall = CreateBall(0.0f, 0.65f, Color.white, 1, "White", false);
        Transform transformBall = gameObjectBall.transform;
        m_ballBehavior_whiteBall = gameObjectBall.GetComponent<BallBehavior>();

        // Cue
        Vector3 quePosition = new Vector3(0.0f, 0.883f, 2.2f);
        Quaternion queRotation = Quaternion.LookRotation((gameObjectBall.transform.position - quePosition).normalized, Vector3.up);
        GameObject gameObjectCue = Instantiate(m_prefab_cue, quePosition, queRotation);
        Transform transformCue = gameObjectCue.transform;
        m_cueBehavior = gameObjectCue.GetComponent<CueBehavior>();
        m_cueBehavior.Initialize(m_cueRotateSpeed, transformBall);

        // Camera
        GameObject gameObjectCamera = Camera.main.gameObject;
        m_cameraBehavior = gameObjectCamera.GetComponent<CameraBehavior>();
        m_cameraBehavior.Initialize(transformCue, transformBall);
    }
    private GameObject CreateBall(float pX, float pZ, Color pColor, int pFull, string pTag, bool pWithOrigin = true) {
        Vector3 origin = new Vector3(0.0f, 0.659f, pWithOrigin ? -0.65f : 0.0f);
        Vector3 position = new Vector3(origin.x + pX, origin.y, origin.z + pZ);
        Quaternion rotation = Quaternion.Euler(UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f), UnityEngine.Random.Range(0.0f, 360.0f));
        GameObject ball = Instantiate(m_prefab_ball, position, rotation);
        ball.tag = pTag;
        BallBehavior behaviorBall = ball.GetComponent<BallBehavior>();
        behaviorBall.Initialize(position, pColor, pFull, m_minVelocity);

        return ball;
    }

    public static void EnqueueEvent(Action<Master> pAction) {
        instance.m_queueEvents.Enqueue(pAction);
    }
    private Action<Master> DequeueEvent() {
        Action<Master> result = null;
        if (m_queueEvents.Count > 0) {
            result = m_queueEvents.Dequeue();
        }
        return result;
    }
    private void ProcessEvents() {
        Action<Master> action = DequeueEvent();
        while (action != null) {
            action(this);
            action = DequeueEvent();
        }
    }

    public void OnClick_StartGame() {
        m_button_startGame.SetActive(false);
        m_panel_mainMenu.SetActive(false);
        m_panel_inGame.SetActive(true);

        Initialize_PhysicsMaterials();
        Initialize_Objects();

        m_playerOne = new Player();
        m_playerTwo = new Player();

        m_currentPlayer = m_playerTwo;
        m_otherPlayer = m_playerOne;

        NextPlayer();

        m_ballBehavior_whiteBall.ReadyUp();
        m_cueBehavior.ReadyUp();
        m_cameraBehavior.ReadyUp();
    }

    public void OnPress_ESC() {
        m_panel_mainMenu.SetActive(!m_panel_mainMenu.activeSelf);
        m_panel_inGame.SetActive(!m_panel_inGame.activeSelf);
    }

    public void OnClick_Settings() {
        m_panel_mainMenu.SetActive(false);
        m_panel_inGame.SetActive(false);
        m_panel_settings.SetActive(true);
    }

    public void OnClick_Ok() {
        m_panel_inGame.SetActive(false);
        m_panel_settings.SetActive(false);
        m_panel_mainMenu.SetActive(true);
    }

    public void OnClick_Quit() {
        EnqueueEvent((pMaster) => {
            Application.Quit();
        });
    }

    public void OnValueChanged_Power() {
        m_text_power.text = $"{m_slider_power.value} %";
    }

    public void OnClick_Shoot() {
        if (m_slider_power.value > 0.0f) {
            m_ballBehavior_whiteBall.Shoot(m_cueBehavior.transform.forward * Power);

            StartCoroutine(DelayMovementCheck());
        }
    }

    public IEnumerator DelayMovementCheck() {
        m_panel_power.SetActive(false);
        yield return new WaitForSecondsRealtime(1.0f);
        m_checkMovement = true;
    }

    public void AddPlayerPoint(int pPlayerID) {
        switch (pPlayerID) {
            case 0:
                m_playerOne.RemainingBalls -= 1;
                break;
            case 1:
                m_playerOne.RemainingBalls -= 1;
                break;
            default: break;
        }
    }

    public void UpdatePlayerScore() {
        m_text_playerOneBalls.text = $"{m_playerOne.RemainingBalls}";
        m_text_playerTwoBalls.text = $"{m_playerTwo.RemainingBalls}";
    }

    public void BallPotted(BallBehavior pBehaviorBall) {
        string tag = pBehaviorBall.gameObject.tag;
        int kind = pBehaviorBall.GetKind();
        if (tag == "Ball") {
            if (m_playerOne.Kind == -1 &&
                m_playerTwo.Kind == -1) {
                m_currentPlayer.Kind = kind;
                m_otherPlayer.Kind = kind == 0 ? 1 : 0;
                m_text_playerOneKind.text = m_playerOne.Kind == 0 ? "Half" : "Full";
                m_text_playerTwoKind.text = m_playerTwo.Kind == 0 ? "Half" : "Full";
            }
            if (m_currentPlayer.Kind != kind) {
                m_otherKindPotted = true;
            }
            if (m_playerOne.Kind == kind) {
                m_playerOne.RemainingBalls -= 1;
            }
            else if (m_playerTwo.Kind == kind) {
                m_playerTwo.RemainingBalls -= 1;
            }
            m_ballBehavior_balls.Remove(pBehaviorBall);
            Destroy(pBehaviorBall.gameObject);
            m_ballPotted = true;
        } else if (tag == "White") {
            m_whiteBallPotted = true;
        } else if (tag == "Black") {
            m_blackBallPotted = true;
        }
        UpdatePlayerScore();
    }

    public void NextRound() {
        if (m_blackBallPotted) {
            GameOver();
        } else if (!m_ballPotted || m_otherKindPotted || m_whiteBallPotted) {
            StartCoroutine(Foul());
            NextPlayer();
        }
        if (m_whiteBallPotted) {
            ResetWhiteBall();
        }

        m_checkMovement = false;
        m_otherKindPotted = false;
        m_whiteBallPotted = false;
        m_blackBallPotted = false;

        if (!m_blackBallPotted) {
            m_panel_power.SetActive(true);
            m_cueBehavior.ReadyUp();
            m_cameraBehavior.ReadyUp();
        }
    }

    public void NextPlayer() {
        Player temp = m_currentPlayer;
        m_currentPlayer = m_otherPlayer;
        m_otherPlayer = temp;

        m_text_playerOne.color = m_currentPlayer == m_playerOne ? Orange : Color.black;
        m_text_playerTwo.color = m_otherPlayer == m_playerTwo ? Color.black : Orange;
    }

    public void ResetWhiteBall() {
        m_ballBehavior_whiteBall.ReadyUp();
    }

    private IEnumerator Foul() {
        m_panel_foul.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        m_panel_foul.SetActive(false);
    }

    public void GameOver() {
        m_cameraBehavior.Play = false;
        m_cueBehavior.Play = false;
        m_ballBehavior_balls.ForEach(behaviorBall => behaviorBall.Play = false);
        m_ballBehavior_whiteBall.Play = false;

        EnqueueEvent((pMaster) => {
            Destroy(m_cameraBehavior.gameObject);
            Destroy(m_cueBehavior.gameObject);
            m_ballBehavior_balls.ForEach(behaviorBall => Destroy(behaviorBall.gameObject));
            Destroy(m_ballBehavior_whiteBall.gameObject);
        });

        if (m_currentPlayer.RemainingBalls > 0) {
            // Lose
            m_panel_loss.SetActive(true);
        } else {
            // Win
            m_panel_win.SetActive(true);
        }
    }
}
