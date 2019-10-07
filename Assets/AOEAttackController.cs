using UnityEngine.Events;
using UnityEngine;

public enum AOEState
{
    IDLE = 0,
    GROW = 1,
    STAY = 2,
    SHRINK = 3
}

public class AOEAttackController : MonoBehaviour
{
    [SerializeField] public AOEState State { get { return _state;  } }

    [SerializeField] public Transform rootTransform;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public CircleCollider2D circleCollider;

    [SerializeField] public float minSize = 0.1f;
    [SerializeField] public float maxSize = 1.0f;

    [SerializeField] public float errorMargin = 0.025f;

    [SerializeField] public float growthDampening = 50f;
    [SerializeField] public float shrinkDampening = 25f;
    [SerializeField] public float stayDuration = 3.0f;

    [SerializeField] private AOEState _state = AOEState.IDLE;
    [SerializeField] private bool _didHitPlayer = false;
    [SerializeField] private float _scaleDampenVelocity = 0f;
    [SerializeField] private float _currentScale = 0f;
    [SerializeField] private float _stayTicker = 0f;

    private void Start() {
        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == AOEState.GROW) {
            _currentScale = Mathf.SmoothDamp(_currentScale, maxSize, ref _scaleDampenVelocity, growthDampening * Time.deltaTime);
            setScales();
            if (_currentScale >= (maxSize - errorMargin)) {
                _state = AOEState.STAY;
                _stayTicker = 0f;
                _scaleDampenVelocity = 0f;
            }
        } else if (_state == AOEState.STAY) {
            if (_stayTicker < stayDuration) {
                _stayTicker += Time.deltaTime;
            }

            if (_stayTicker >= stayDuration) {
                _state = AOEState.SHRINK;
            }
        } else if (_state == AOEState.SHRINK) {
            _currentScale = Mathf.SmoothDamp(_currentScale, minSize, ref _scaleDampenVelocity, shrinkDampening * Time.deltaTime);
            setScales();
            if (_currentScale <= (minSize + errorMargin)) {
                Deactivate();
            }
        }
    }

    public void Activate() {
        _state = AOEState.GROW;
        _currentScale = minSize;
        _didHitPlayer = false;
        _scaleDampenVelocity = 0f;
        setScales();
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
    }

    public void Deactivate() {
        _state = AOEState.IDLE;
        _currentScale = minSize;
        _scaleDampenVelocity = 0f;
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        _didHitPlayer = false;
        setScales();
    }

    private void setScales() {
        rootTransform.localScale = new Vector3(_currentScale, _currentScale, rootTransform.localScale.z);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (!_didHitPlayer) {
                Debug.Log("oof bud");
                _didHitPlayer = true;
            }
        }
    }
}
