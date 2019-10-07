using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperMovementController : MonoBehaviour
{
    public bool IsAtHoverHeight { get { return Mathf.Abs(thumperSettings.hoverHeight - transform.position.y) < thumperSettings.hoverErrorMargin; } }

    // Last direction of movement (by input)
    public float LastDirection { get { return _lastDirection; } }

    public bool IsBlocked { get { return _isBlocked; } }

    // Is the player colliding down
    public bool IsGrounded { get { return _isGrounded; } }

    // Is the controller colliding upw
    public bool IsTouchingCeiling { get { return _isTouchingCeiling; } }

    // Int defines direction of wall, 0 means none
    public int TouchingWallDirection { get { return _touchingWallDirection; } }

    // Rigidbody's current velocity
    public Vector2 Velocity { get { return rigidbody.velocity; } }

    [SerializeField] public GroundMovementSettings movementSettings;
    [SerializeField] public ThumperSettings thumperSettings;
    [SerializeField] public new Collider2D collider;
    [SerializeField] public new Rigidbody2D rigidbody;

    [SerializeField] private Transform _transform;
    [SerializeField] private RaycastHit2D[] _contacts = new RaycastHit2D[3];
    [SerializeField] private Vector2 _currentVelocity = Vector2.zero;
    [SerializeField] private bool _shouldJump = false;
    [SerializeField] private float _lastDirection = 1f;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private int _touchingWallDirection = 0;
    [SerializeField] private bool _isTouchingCeiling = false;
    [SerializeField] private bool _didMoveLastFrame = false;
    [SerializeField] private bool _isBlocked = false;


    public void Start() {
        if (!_transform) _transform = GetComponent<Transform>();
        if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
        if (!collider) collider = GetComponent<BoxCollider2D>();

        rigidbody.isKinematic = false;
        rigidbody.simulated = true;
        rigidbody.freezeRotation = true;
    }

    public void FixedUpdate() {
        updateTouchingWalls();

        if (!_didMoveLastFrame) {
            dampenMovement();
        }
        _didMoveLastFrame = false;

        hover();

        updateTouchingCeiling();
        updateBlocked();
    }

    public void Move(float direction) {
        _lastDirection = Mathf.Sign(rigidbody.velocity.x);

        // prevent wall sticking
        if (Mathf.Sign(direction) == _touchingWallDirection) {
            direction = 0;
        }

        // prevent affecting velocity with move when input is 0
        _didMoveLastFrame = direction > 0f || direction < 0f;
        if (!_didMoveLastFrame) return;

        if (_isGrounded) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.groundMoveVelocity, rigidbody.velocity.y);
            rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundMoveSmoothing);
        } else if (movementSettings.canMoveInAir) {
            float desiredDirection = Mathf.Sign(direction);
            Vector2 targetVelocity = new Vector2(direction * movementSettings.airMoveVelocity, rigidbody.velocity.y);
            rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airMoveSmoothing);
        }
    }

    public void Update() {
        if (_shouldJump) {
            _shouldJump = false;
            _isGrounded = false;
            rigidbody.AddForce(new Vector2(0f, movementSettings.jumpForce));
        }
    }

    public void ClearVelocity() {
        rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, Vector2.zero, ref _currentVelocity, 0.0001f);
    }

    public void ClearVelocityX() {
        rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, new Vector2(0, rigidbody.velocity.y), ref _currentVelocity, 0.0001f);
    }

    public void AddForce(Vector2 force) {
        rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

    private void hover() {
        float heightDifference = thumperSettings.hoverHeight - transform.position.y;
        Vector2 targetVelocity = Vector2.zero;
        if (heightDifference > thumperSettings.hoverErrorMargin) {
            // go up
            targetVelocity = new Vector2(rigidbody.velocity.x, thumperSettings.hoverHeightSpeed);
        } else if (heightDifference < -thumperSettings.hoverErrorMargin) {
            // go down
            targetVelocity = new Vector2(rigidbody.velocity.x, -thumperSettings.hoverHeightSpeed);
        } else {
            // hover here
            targetVelocity = new Vector2(rigidbody.velocity.x, 0);
        }
        rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, thumperSettings.hoverHeightSmoothing);
    }

    private void dampenMovement() {
        if (_isGrounded) {
            Vector2 targetVelocity = new Vector2(0, rigidbody.velocity.y);
            if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
            rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.groundStopSmoothing);

        } else if (movementSettings.dampenAirMovement) {
            Vector2 targetVelocity = new Vector2(0, rigidbody.velocity.y);
            if (_currentVelocity.x < 0.01f) _currentVelocity.Set(0, _currentVelocity.y);
            rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref _currentVelocity, movementSettings.airStopSmoothing);
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
       // _isGrounded = isTouching(Vector2.down, movementSettings.minGroundDistance, movementSettings.groundLayer);
    }

    private void updateBlocked() {
        ContactFilter2D filter = new ContactFilter2D();
        filter.ClearLayerMask();

        int contactCount = collider.Cast(Vector2.down, filter, _contacts, movementSettings.minGroundDistance);
        for (int i = 0; i < contactCount; i++) {
            if (_contacts[i].collider != null && _contacts[i].transform != transform) {
                _isBlocked = true;
                if (_contacts[i].collider.tag == "Ground") {
                    _isGrounded = true;
                } else {
                    _isGrounded = false;
                }
                return;
            }
        }
        _isGrounded = false;
        _isBlocked = false;
    }

    private bool isTouching(Vector2 direction, float distance, int mask = 1 << 0) {
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = mask;


        int contactCount = collider.Cast(direction, filter, _contacts, distance);
        for (int i = 0; i < contactCount; i++) {
            if (_contacts[i].collider != null) {
                return true;
            }
        }
        return false;
    }
}
