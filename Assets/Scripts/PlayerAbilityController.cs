﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    public bool IsSlamming { get { return _isSlamming; } }

    [Header("General")]
    [SerializeField] public LayerMask enemyLayerMask;
    [SerializeField] public PlayerController controller;

    [Header("Kick (Passive)")]
    [SerializeField] public bool hasKick = true;

    [Header("Punch")]
    [SerializeField] public bool hasPunch = true;
    [SerializeField] private float _punchDistance = 1.5f;
    [SerializeField] private float _punchLungeForce = 0.5f;

    [Header("Slam")]
    [SerializeField] public bool hasSlam = true;
    [SerializeField] private float _slamForce = 10f;
    [SerializeField] private bool _isSlamming = false;

    [Header("Wall Jump")]
    [SerializeField] public bool enableWallJump = true;

    [SerializeField] private Collider2D[] _overlaps = new Collider2D[4];
    [SerializeField] private RaycastHit2D[] _hits = new RaycastHit2D[4];
    [SerializeField] private bool _waitForNotBlocked = false;

    public void Update() {
        controller.movementController.movementSettings.enableWallJump = enableWallJump;
        if (_isSlamming && controller.movementController.IsBlocked) _isSlamming = false;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (controller.movementController.IsGrounded) return;

        if (_waitForNotBlocked) {
            if (controller.movementController.IsBlocked) return;
            else _waitForNotBlocked = false;
        }


        int contacts = Physics2D.CircleCastNonAlloc(transform.position, 0.99f, controller.movementController.Velocity.normalized, _hits, 0.25f, enemyLayerMask);
        for (int i = 0; i < contacts; i++) {
            if (_hits[i].collider.gameObject.tag == "Enemy") {
                Vulnerable consumer = _hits[i].collider.gameObject.GetComponent<Vulnerable>();
                if (consumer) {
                    consumer.RecieveAttack(_isSlamming ? AttackType.Slam : AttackType.Jump, _hits[i].point);
                    _waitForNotBlocked = true;
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
                    consumer.RecieveAttack(AttackType.Punch, _hits[i].point);
                    Debug.DrawLine(transform.position, consumer.transform.position, Color.red, 2.0f);
                    return true;
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position + ((Vector3)direction * _punchDistance), Color.blue, 2.0f);
        return true;
    }

    public bool Slam() {
        if (!hasSlam || controller.movementController.IsGrounded) return false;
        controller.movementController.ClearVelocity();
        Vector2 direction = new Vector2(0, -_slamForce);
        controller.movementController.AddForce(direction);
        _isSlamming = true;
        return true;
    }
}
