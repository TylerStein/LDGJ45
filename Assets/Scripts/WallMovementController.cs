using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallMovementController : MonoBehaviour
{
    [Tooltip("Side of the transform to stick with and move along")]
    [SerializeField] public StickSide stickSide = StickSide.LEFT;
    [SerializeField] public float castColliderSize = 0.99f;

    [SerializeField] public WallMovementSettings movementSettings;
    [SerializeField] public new Rigidbody2D rigidbody;
    [SerializeField] public new Transform transform;
    [SerializeField] public new Collider2D collider;

    public int LastDirection { get { return _lastMoveDirection; } }
    public bool IsOnWall { get { return _isOnWall; } }
    public bool IsBlocked { get { return _isBlocked;  } }
    
    [SerializeField] private float _velocity = 0f;
    [SerializeField] private float _dampeningVelocity = 0f;
    [SerializeField] private int _lastMoveDirection = 0;
    [SerializeField] private bool _isOnWall;
    [SerializeField] private bool _isBlocked;

    [SerializeField] private Vector2 _right = Vector2.right;
    [SerializeField] private Vector2 _up = Vector2.up;

    [SerializeField] private StickSide lastStickSide = StickSide.LEFT;
    [SerializeField] private RaycastHit2D[] _hits = new RaycastHit2D[4];

    // Start is called before the first frame update
    void Start()
    {
        if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
        if (!transform) transform = GetComponent<Transform>();
        if (!collider) collider = GetComponent<Collider2D>();

        rigidbody.isKinematic = true;
        rigidbody.freezeRotation = true;
        lastStickSide = stickSide;
        calculateDirections();
    }

    void FixedUpdate() {
        if (stickSide != lastStickSide) {
            lastStickSide = stickSide;
            calculateDirections();
        }

        applyMovement();
    }

    public void Move(float direction) {
        float desiredDirection = Mathf.Sign(direction);
        _velocity = Mathf.SmoothDamp(_velocity, desiredDirection * movementSettings.surfaceMoveVelocity, ref _dampeningVelocity, movementSettings.surfaceMoveSmoothing);
    }

    private void applyMovement() {

        // Stick to surface
        int contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * castColliderSize, 0, _up * -1, _hits, movementSettings.surfacePull, movementSettings.surfaceLayerMask);
        float maxMoveDistance = movementSettings.surfacePull;
        for (int i = 0; i < contacts; i++) {
            if (_hits[i].collider != collider) {
                maxMoveDistance = _hits[i].distance;
                break;
            }
        }
        float moveDistance = Mathf.Clamp(maxMoveDistance, 0, movementSettings.surfacePull * Time.deltaTime);
        Vector2 moveDirection = _up * -1;
        if (moveDistance >= movementSettings.minWallDistance) {
            transform.position += (Vector3)moveDirection * (moveDistance - movementSettings.minWallDistance * 0.9f);
            _isOnWall = false;
        } else {
            _isOnWall = true;
        }

        if (_isOnWall) {
            // Slide along surface
            float absVelocity = Mathf.Abs(_velocity);
            moveDirection = _velocity > 0 ? _right : _right * -1f;
            contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * castColliderSize, 0, moveDirection, _hits, absVelocity, movementSettings.surfaceLayerMask);
            maxMoveDistance = absVelocity;
            for (int i = 0; i < contacts; i++) {
                if (_hits[i].collider != collider) {
                    maxMoveDistance = _hits[i].distance;
                    break;
                }
            }
            _isBlocked = (maxMoveDistance < absVelocity);
            moveDistance = Mathf.Clamp(maxMoveDistance, 0, absVelocity);
            transform.position += (Vector3)moveDirection * Time.deltaTime * moveDistance;
        }
    }

    private void calculateDirections() {
        switch (stickSide) {
            case StickSide.CEILING:
                _up = Vector2.down;
                _right = Vector2.left;
                break;
            case StickSide.LEFT:
                _up = Vector2.right;
                _right = Vector2.down;
                break;
            case StickSide.RIGHT:
                _up = Vector2.left;
                _right = Vector2.up;
                break;
        }
    }
}
