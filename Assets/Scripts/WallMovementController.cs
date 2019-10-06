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

    public int LastDirection { get { return _lastMoveDirection; } }

    [SerializeField] private float _forces = 0f;
    [SerializeField] private float _velocityAdd = 0f;
    [SerializeField] private float _velocity = 0f;
    [SerializeField] private float _dampeningVelocity = 0f;
    [SerializeField] private int _lastMoveDirection = 0;

    [SerializeField] private Vector2 _right = Vector2.right;
    [SerializeField] private Vector2 _up = Vector2.up;

    [SerializeField] private StickSide lastStickSide = StickSide.LEFT;
    [SerializeField] private RaycastHit2D[] _hits = new RaycastHit2D[4];

    // Start is called before the first frame update
    void Start()
    {
        rigidbody.isKinematic = true;
        rigidbody.freezeRotation = true;
        lastStickSide = stickSide;
        calculateDirections();
    }

    // Update is called once per frame
    void Update() {
        if (stickSide != lastStickSide) {
            lastStickSide = stickSide;
            calculateDirections();
        }

        calculateVelocity();

        applyMovement();
    }

    public void Move(float force) {
        _forces += force;
    }

    public void AddVelocity(float velocity) {
        _velocityAdd += velocity;
    }

    public void SetVelocity(float velocity) {
        _velocity = 0f;
    }

    private void calculateVelocity() {
        float targetVelocity = _velocity + _velocityAdd + (_forces * Time.deltaTime);
        _velocity = Mathf.SmoothDamp(_velocity, targetVelocity, ref _dampeningVelocity, movementSettings.surfaceMoveSmoothing);
        _forces = 0f;
        _velocityAdd = 0f;
    }

    private void applyMovement() {

        // Stick to surface
        int contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * castColliderSize, 0, _up * -1, _hits, movementSettings.surfacePull);
        float maxMoveDistance = movementSettings.surfacePull;
        if (contacts > 0) {
            maxMoveDistance = _hits[0].distance;
        }
        float moveDistance = Mathf.Clamp(maxMoveDistance, 0, movementSettings.surfacePull);
        Vector2 moveDirection = _up * -1;
        transform.position += (Vector3)moveDirection * Time.deltaTime * moveDistance;

        // Slide along surface
        float absVelocity = Mathf.Abs(_velocity);
        moveDirection = _velocity > 0 ? _right : _right * -1f;
        contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * castColliderSize, 0, moveDirection, _hits, absVelocity);
        maxMoveDistance = absVelocity;
        if (contacts > 0) {
            maxMoveDistance = _hits[0].distance;
        }
        moveDistance = Mathf.Clamp(maxMoveDistance, 0, absVelocity);
        transform.position += (Vector3)moveDirection * Time.deltaTime * moveDistance;
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
