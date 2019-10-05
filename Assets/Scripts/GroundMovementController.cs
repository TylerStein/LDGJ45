using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GroundMovementController : MonoBehaviour
{
    // Last direction of movement (by input)
    public float LastDirection { get { return _lastDirection; } }

    // Is the player colliding down
    public bool IsGrounded { get { return _isGrounded; } }

    // Is the controller colliding upw
    public bool IsTouchingCeiling { get { return _isTouchingCeiling; } }

    // Int defines direction of wall, 0 means none
    public int TouchingWallDirection { get { return _touchingWallDirection; } }


    [SerializeField] public GroundMovementSettings movementSettings;

    [SerializeField] private Transform _transform;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private RaycastHit2D[] _contacts = new RaycastHit2D[3];
    [SerializeField] private Vector2 _currentVelocity = Vector2.zero;
    [SerializeField] private bool _shouldJump = false;
    [SerializeField] private float _lastDirection = 1f;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private int _touchingWallDirection = 0;
    [SerializeField] private bool _isTouchingCeiling = false;
    [SerializeField] private bool _didMoveLastFrame = false;


    public void Start() {
        if (!_transform) _transform = GetComponent<Transform>();
        if (!_rigidbody) _rigidbody = GetComponent<Rigidbody2D>();
        if (!_collider) _collider = GetComponent<BoxCollider2D>();

        _rigidbody.isKinematic = false;
        _rigidbody.simulated = true;
        _rigidbody.freezeRotation = true;
    }

    public void FixedUpdate() {
        updateTouchingWalls();

        if (!_didMoveLastFrame) {
            dampenMovement();
        }
        _didMoveLastFrame = false;

        updateTouchingCeiling();
        updateTouchingGround();
    }


    public void Jump(bool canJumpInAir = false) {
        if ((_isGrounded || canJumpInAir) && !_isTouchingCeiling) _shouldJump = true;
    }

    public void Move(float direction) {
        _lastDirection = Mathf.Sign(_rigidbody.velocity.x);

        // prevent wall sticking
        if (Mathf.Sign(direction) == _touchingWallDirection) {
            direction = 0;
        }

        // prevent affecting velocity with move when input is 0
        _didMoveLastFrame = direction > 0f || direction < 0f;
        if (!_didMoveLastFrame) return;

        if (_isGrounded) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.groundMoveVelocity, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundMoveSmoothing);
        } else if (movementSettings.canMoveInAir) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.airMoveVelocity, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airMoveSmoothing);
        }
    }

    public void Update() {
        if (_shouldJump) {
            _shouldJump = false;
            _isGrounded = false;
            _rigidbody.AddForce(new Vector2(0f, movementSettings.jumpForce));
        }
    }


    public void AddForce(Vector2 force) {
        _rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    private void dampenMovement() {
        if (_isGrounded) {
           Vector2 targetVelocity = new Vector2(0, _rigidbody.velocity.y);
           if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
           _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundStopSmoothing);

        } else if (movementSettings.canMoveInAir) {
           Vector2 targetVelocity = new Vector2(0, _rigidbody.velocity.y);
            if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
            _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airStopSmoothing);
        }
    }

    private void updateTouchingWalls() {
        if (_lastDirection < 0) {
            // looking left, test left first
            if (isTouching(Vector2.left, movementSettings.minWallDistance, movementSettings.wallLayer)) {
                _touchingWallDirection = -1;
            } else if (isTouching(Vector2.right, movementSettings.minWallDistance, movementSettings.wallLayer)) {
                _touchingWallDirection = 1;
            } else {
                _touchingWallDirection = 0;
            }
        } else {
            // looking right, test right first
            if (isTouching(Vector2.right, movementSettings.minWallDistance, movementSettings.wallLayer)) {
                _touchingWallDirection = 1;
            } else if (isTouching(Vector2.left, movementSettings.minWallDistance, movementSettings.wallLayer)) {
                _touchingWallDirection = -1;
            } else {
                _touchingWallDirection = 0;
            }
        }
    }

    private void updateTouchingCeiling() {
        _isTouchingCeiling = isTouching(Vector2.up, movementSettings.minCeilingDistance, movementSettings.ceilingLayer);
    }

    private void updateTouchingGround() {
        _isGrounded = isTouching(Vector2.down, movementSettings.minGroundDistance, movementSettings.groundLayer);
    }

    private bool isTouching(Vector2 direction, float distance, int mask = 1 << 0) {
        int contactCount = Physics2D.BoxCastNonAlloc(_transform.position, Vector2.one * 0.99f, 0, direction, _contacts, distance, mask);
        for (int i = 0; i < contactCount; i++) {
            if (_contacts[i].collider != null) {
                return true;
            }
        }
        return false;
    }
}
