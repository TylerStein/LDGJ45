using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    PREGAME = 0,
    INGAME = 1,
    LOSE = 2,
    WIN = 3,
    POSTWIN = 4
}

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get { return _instance; } }
    private static GameStateController _instance;

    public bool IsPlaying { get { return _gameState == GameState.INGAME;  } }

    [SerializeField] public List<EnemyController> _enemies = new List<EnemyController>();
    [SerializeField] public PlayerInputProvider inputProvider;

    [SerializeField] public GameObject inGameUI;
    [SerializeField] public GameObject preGameUI;
    [SerializeField] public GameObject postWinUI;
    [SerializeField] public GameObject postLoseUI;

    [SerializeField] private Transform _elevatorTransform;
    [SerializeField] private Collider2D _ceilingCollider;
    [SerializeField] private Vector2 _winStateElevatorTarget = new Vector2(0, 15f);
    [SerializeField] private float _winStateElevatorSmoothing = 50f;
    [SerializeField] private Vector2 _winStateElevatorCurrentVelocity = Vector2.zero;

    [SerializeField] private int gameSceneIndex = 0;
    [SerializeField] private GameState _gameState = GameState.PREGAME;
    [SerializeField] private bool _pendingStartGame = false;

    public void Awake() {
        GameStateController._instance = this;
        if (_enemies.Count == 0) _enemies = new List<EnemyController>(FindObjectsOfType<EnemyController>());
        preGameUI.SetActive(true);
        inGameUI.SetActive(false);
        postWinUI.SetActive(false);
        postLoseUI.SetActive(false);
        _gameState = GameState.PREGAME;
    }

    public void Update() {
        if (_gameState == GameState.PREGAME) {
            if (_pendingStartGame) {
                _gameState = GameState.INGAME;
            } else if (inputProvider.AnyInput) {
                preGameUI.SetActive(false);
                inGameUI.SetActive(true);
                _pendingStartGame = true;
            }
        } if (_gameState == GameState.INGAME) {
            if (Application.isEditor) {
                // Editor only inputs
                if (Input.GetKeyDown(KeyCode.Equals)) {
                    Win();
                } else if (Input.GetKeyDown(KeyCode.Minus)) {
                    FindObjectOfType<PlayerController>().Die();
                    Lose();
                }
            }
        } else if (_gameState == GameState.WIN) {
            _elevatorTransform.position = Vector2.SmoothDamp(_elevatorTransform.position, _winStateElevatorTarget, ref _winStateElevatorCurrentVelocity, _winStateElevatorSmoothing * Time.deltaTime);
            if (_elevatorTransform.position.y >= (_winStateElevatorTarget.y - 2.0f)) {
                postWinUI.SetActive(true);
                _gameState = GameState.POSTWIN;
            }
        } else if (_gameState == GameState.POSTWIN) {
            if (inputProvider.AnyInput) {
                SceneManager.LoadScene(gameSceneIndex);
            }
        } else if (_gameState == GameState.LOSE) {
            if (inputProvider.AnyInput) {
                SceneManager.LoadScene(gameSceneIndex);
            }
        }
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

    public void Win() {
        _gameState = GameState.WIN;
        _ceilingCollider.enabled = false;

        //for (int i = 0; i < _enemies.Count; i++) Destroy(_enemies[i].gameObject, 0.01f);
        //_enemies.Clear();

        inGameUI.SetActive(false);
    }

    public void Lose() {
        _gameState = GameState.LOSE;
        inGameUI.SetActive(false);
        postLoseUI.SetActive(true);
    }
}
