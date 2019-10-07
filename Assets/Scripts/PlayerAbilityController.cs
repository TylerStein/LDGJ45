using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    public bool IsSlamming { get { return _isSlamming; } }
    public Collider2D attackCollider;

    [Header("General")]
    [SerializeField] public LayerMask enemyLayerMask;
    [SerializeField] public PlayerController controller;

    [Header("Kick (Passive)")]
    [SerializeField] public bool hasKick = true;

    [Header("Punch")]
    [SerializeField] public bool hasPunch = true;
    [SerializeField] private float _punchDistance = 1.5f;
    [SerializeField] private float _punchLungeForce = 0.5f;
    [SerializeField] public float _punchCooldown = 0.5f;

    [Header("Slam")]
    [SerializeField] public bool hasSlam = true;
    [SerializeField] private float _slamForce = 10f;
    [SerializeField] private bool _isSlamming = false;

    [Header("Wall Jump")]
    [SerializeField] public bool enableWallJump = true;

    [SerializeField] private Collider2D[] _overlaps = new Collider2D[4];
    [SerializeField] private RaycastHit2D[] _hits = new RaycastHit2D[4];
    [SerializeField] private bool _waitForNotBlocked = false;
    [SerializeField] private bool _waitForGrounded = false;

    public void Update() {
        controller.movementController.movementSettings.enableWallJump = enableWallJump;
        if (_isSlamming && controller.movementController.IsBlocked) _isSlamming = false;
        if (controller.movementController.IsGrounded) _waitForGrounded = false;
        if (controller.movementController.IsBlocked) _waitForNotBlocked = false;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (_waitForGrounded || _waitForNotBlocked || controller.movementController.IsGrounded) {
            return;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = enemyLayerMask;

        int contacts = attackCollider.Cast(controller.movementController.Velocity.normalized, filter, _hits, 0.25f);
        for (int i = 0; i < contacts; i++) {
            if (_hits[i].collider.gameObject.tag == "Enemy") {
                Vulnerable consumer = _hits[i].collider.gameObject.GetComponent<Vulnerable>();
                if (consumer) {
                    consumer.RecieveAttack(_isSlamming ? AttackType.Slam : AttackType.Jump, attackCollider, _hits[i]);
                    if (_isSlamming) _waitForNotBlocked = true;
                    else _waitForGrounded = true;
                    break;
                }
            }
        }

        _isSlamming = false;
    }

    public bool Punch() {
        if (!hasPunch) return false;
        Vector2 direction = Vector2.right * controller.movementController.LastDirection;
        controller.movementController.AddForce(direction * _punchLungeForce);
        int contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * 0.75f, 0, direction, _hits, _punchDistance, enemyLayerMask);
        for (int i = 0; i < contacts; i++) {
            if (_hits[i].collider.gameObject.tag == "Enemy") {
                Vulnerable consumer = _hits[i].collider.gameObject.GetComponent<Vulnerable>();
                if (consumer) {
                    consumer.RecieveAttack(AttackType.Punch, attackCollider, _hits[i]);
                    Debug.DrawLine(transform.position, consumer.transform.position, Color.red, 2.0f);
                    return true;
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position + ((Vector3)direction * _punchDistance), Color.blue, 2.0f);
        return true;
    }

    public bool Slam() {
        if (!hasSlam || controller.movementController.IsBlocked || controller.movementController.IsGrounded) return false;
        controller.movementController.ClearVelocity();
        Vector2 direction = new Vector2(0, -_slamForce);
        controller.movementController.AddForce(direction);
        _isSlamming = true;
        return true;
    }
}
