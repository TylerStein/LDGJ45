using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PREGAME = 0,
    ENTERGAME = 1,
    INGAME = 2,
    LOSE = 3,
    WIN = 4,
    POSTWIN = 5
}

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get { return _instance; } }
    private static GameStateController _instance;

    public bool IsPlaying { get { return _gameState == GameState.INGAME;  } }

    [SerializeField] public List<EnemyController> _enemies = new List<EnemyController>();
    [SerializeField] public PlayerInputProvider inputProvider;
    [SerializeField] public CameraController cameraController;
    [SerializeField] public PlayerController player;

    [SerializeField] public GameObject inGameUI;
    [SerializeField] public GameObject preGameUI;
    [SerializeField] public GameObject postWinUI;
    [SerializeField] public GameObject postLoseUI;

    [SerializeField] private Transform _elevatorTransform;
    [SerializeField] private Transform _worldRoot;

    [SerializeField] private Collider2D _ceilingCollider;

    [SerializeField] private Color[] enemyColors;

    [SerializeField] private Vector2 _winStateWorldTarget = new Vector2(0f, -20f);
    [SerializeField] private Vector2 _inGameWorldTarget = new Vector2(0f, 0f);
    [SerializeField] private Vector2 _preGameWorldPosition = new Vector2(0, 20f);

    [SerializeField] private Vector2 _worldTargetVelocity = Vector2.zero;
    [SerializeField] private float _worldTargetSmoothing = 50f;
    [SerializeField] private float _worldTargetSnapDistance = 0.15f;

    [SerializeField] private int gameSceneIndex = 0;
    [SerializeField] private GameState _gameState = GameState.PREGAME;
    [SerializeField] private bool _pendingStartGame = false;

    public void Awake() {
        GameStateController._instance = this;
        if (_enemies.Count == 0) _enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        if (!player) player = FindObjectOfType<PlayerController>();

        if(PlayerPrefs.GetInt("hasWon", 0) != 0)
        {
            NewGamePlusSetup();
        }

        // init ui state
        preGameUI.SetActive(true);
        inGameUI.SetActive(false);
        postWinUI.SetActive(false);
        postLoseUI.SetActive(false);

        PrepareForAnimation();
        _worldRoot.position = _preGameWorldPosition;

        _gameState = GameState.PREGAME;
    }

    public void Update() {
        if (inputProvider.EscapeDown && Application.platform != RuntimePlatform.WebGLPlayer) {
            Application.Quit();
        }

        if (_gameState == GameState.PREGAME) {
            // pre-game menu
            if (inputProvider.StartDown) {
                preGameUI.SetActive(false);
                inGameUI.SetActive(true);
                PrepareForAnimation();
                _worldRoot.position = _preGameWorldPosition;
                _gameState = GameState.ENTERGAME;
            }
        } else if (_gameState == GameState.ENTERGAME) {
            // pre-game animation
            if (_pendingStartGame) {
                _gameState = GameState.INGAME;
            } else {
                if (Mathf.Abs(_worldRoot.position.y  - _inGameWorldTarget.y) <= _worldTargetSnapDistance) {
                    _worldRoot.position = Vector2.SmoothDamp(_worldRoot.position, _inGameWorldTarget, ref _worldTargetVelocity, _worldTargetSmoothing * 0.15f * Time.deltaTime);
                    if (Mathf.Abs(_worldRoot.position.y - _inGameWorldTarget.y) <= _worldTargetSnapDistance * 0.1f) {
                        _worldRoot.position = _inGameWorldTarget;
                        TeardownAnimation();
                        _pendingStartGame = true;
                    }
                } else {
                    _worldRoot.position = Vector2.SmoothDamp(_worldRoot.position, _inGameWorldTarget, ref _worldTargetVelocity, _worldTargetSmoothing * Time.deltaTime);
                }
            }
        } else if (_gameState == GameState.INGAME) {
            // in-game
            if (Application.isEditor) {
                // Editor only inputs
                if (Input.GetKeyDown(KeyCode.Equals)) {
                    Win();
                } else if (Input.GetKeyDown(KeyCode.Minus)) {
                    player.Die();
                    Lose();
                }
            }
        } else if (_gameState == GameState.WIN) {
            // post-game win animation
            if (Mathf.Abs(_worldRoot.position.y - _winStateWorldTarget.y) <= _worldTargetSnapDistance) {
                _worldRoot.position = Vector2.SmoothDamp(_worldRoot.position, _winStateWorldTarget, ref _worldTargetVelocity, _worldTargetSmoothing * 0.15f * Time.deltaTime);
                if (Mathf.Abs(_worldRoot.position.y - _winStateWorldTarget.y) <= _worldTargetSnapDistance * 0.1f) {
                    postWinUI.SetActive(true);
                   // TeardownAnimation();
                    _gameState = GameState.POSTWIN;
                }
            } else {
                _worldRoot.position = Vector2.SmoothDamp(_worldRoot.position, _winStateWorldTarget, ref _worldTargetVelocity, _worldTargetSmoothing * Time.deltaTime);
            }
        } else if (_gameState == GameState.POSTWIN)
        {
            // post-game win menu RESTART
            if (inputProvider.StartDown)
            {
                PlayerPrefs.SetInt("hasWon", 0);
                PlayerPrefs.Save();
                SceneManager.LoadScene(gameSceneIndex);
            }
            // post-game win menu NEWGAME+
            if (inputProvider.SelectDown)
            {
                PlayerPrefs.SetInt("hasWon", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(gameSceneIndex);
            }
        } else if (_gameState == GameState.LOSE) {
            // post-game lose menu
            if (inputProvider.StartDown) {
                SceneManager.LoadScene(gameSceneIndex);
            }
        }
    }

    private void NewGamePlusSetup()
    {
        List<EnemyController> jumpList = new List<EnemyController>(),
                              punchList = new List<EnemyController>(),
                              slamList = new List<EnemyController>();
        int jump = Random.Range(0, 3),
        punch = Random.Range(0, 3),
        slam = 0;

        if (punch == jump)
        {
            punch = (punch + 1) % 3;
        }
        while (slam == jump || slam == punch)
        {
            slam = (slam + 1) % 3;
        }

        foreach (EnemyController en in _enemies)
        {
            switch (en.vulnerableTo)
            {
                case AttackType.Jump:
                    jumpList.Add(en);
                    break;
                case AttackType.Punch:
                    punchList.Add(en);
                    break;
                case AttackType.Slam:
                    slamList.Add(en);
                    break;
            }
        }

        foreach (EnemyController en in jumpList)
        {
            en.vulnerableTo = (AttackType)jump;
            en.spriteRenderer.color = enemyColors[jump];
        }
        foreach (EnemyController en in punchList)
        {
            en.vulnerableTo = (AttackType)punch;
            en.spriteRenderer.color = enemyColors[punch];
        }
        foreach (EnemyController en in slamList)
        {
            en.vulnerableTo = (AttackType)slam;
            en.spriteRenderer.color = enemyColors[slam];
        }

        player.abilityController.hasPunch = true;
        player.abilityController.hasSlam = true;
    }

    public void OnEnemyDie(EnemyController enemy) {
        int index = _enemies.FindIndex((EnemyController controller) => enemy == controller);
        if (index == -1) Debug.LogWarning("Enemy " + enemy.gameObject.name + " is not tracked in the GameStateController!");
        else {
            _enemies.RemoveAt(index);
            if (_enemies.Count == 0) {
                Win();
            }
        }
    }

    public void OnPlayerDie() {
        Lose();
    }

    public void PrepareForAnimation() {
        // prep for intro sequence
        cameraController.enabled = false;
        _elevatorTransform.parent = null;
        cameraController.transform.parent = _worldRoot;
        _ceilingCollider.enabled = false;
        _worldTargetVelocity = Vector2.zero;
    }

    public void TeardownAnimation() {
        cameraController.enabled = true;
        _elevatorTransform.parent = _worldRoot;
        cameraController.transform.parent = null;
        _ceilingCollider.enabled = true;
        _worldTargetVelocity = Vector2.zero;
    }

    public void Win() {
        _gameState = GameState.WIN;
        PrepareForAnimation();

        inGameUI.SetActive(false);
    }

    public void Lose() {
        _gameState = GameState.LOSE;
        inGameUI.SetActive(false);
        postLoseUI.SetActive(true);
    }
}
