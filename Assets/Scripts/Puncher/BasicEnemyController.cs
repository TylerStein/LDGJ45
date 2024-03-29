﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class BasicEnemyController : EnemyController
{
    [SerializeField] public LayerMask wallLayerMask;
    [SerializeField] public LayerMask attackLayerMask;

    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public ScrapSpawner scrapSpawner;
    [SerializeField] public Enemy_Sound_Controller soundController;
    [SerializeField] private float punchMoveForce = 10.0f;
    [SerializeField] private float punchDistance = 0.7f;
    
    private void Awake()
    {
        if (!movementController) movementController = GetComponent<GroundMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();
        GetComponentInChildren<BoxerHeadVulnerable>().vulnerableTo = vulnerableTo;

        Dictionary<StateType, BaseState> states = new Dictionary<StateType, BaseState>()
        {
            {StateType.Patrol, new GroundPatrolState(this) },
            {StateType.Chase, new GroundChaseState(this) },
            {StateType.Attack, new GroundAttackState(this) }
        };
        stateMachine.SetStates(states);
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
        animator.SetTrigger("Damage");

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        soundController.PlayDie();

        GameStateController.Instance.OnEnemyDie(this);
        movementController.rigidbody.simulated = false;
        movementController.collider.enabled = false;
        movementController.enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1.0f);
        enabled = false;
    }

    public override void GiveAttack()
    {
        Vector2 collideBoxOrigin = new Vector2(movementController.LastDirection * punchDistance + transform.position.x, transform.position.y);
        Collider2D collider = Physics2D.OverlapBox(collideBoxOrigin, new Vector2(0.5f, 0.2f), 0f, attackLayerMask);

        movementController.AddForce(Vector2.right * movementController.LastDirection * punchMoveForce);
        animator.SetTrigger("Attack");

        vfxController.SpawnSwipeVFX(transform.position + new Vector3(movementController.LastDirection/2, 0, 0),
            Vector3.zero, Color.white, movementController.LastDirection == 1 ? true : false);

        soundController.PlayAttack();

        if (collider == null)
            return;

        PlayerController p = collider.gameObject.GetComponent<PlayerController>();
        if (p)
        {
            p.ReceiveAttack(this);
            Debug.DrawLine(transform.position, p.transform.position, Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.Instance.IsPlaying == false) return;

        stateMachine.Update();
    }
}
