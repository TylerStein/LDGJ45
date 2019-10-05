﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class BasicEnemyController : EnemyController
{
    [SerializeField] public LayerMask wallLayerMask;

    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public AwarenessProvider awarenessProvider;

    [SerializeField] private int moveDirection = 1;

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        Vector2 inDirection = (point - (Vector2)awarenessProvider.Player.transform.position).normalized;
        awarenessProvider.Player.movementController.AddForce(Vector2.Reflect(inDirection, Vector2.up) * 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Sign(distanceToPlayer) != moveDirection) {
            moveDirection = moveDirection * -1;
        }
  
        if (distanceToPlayer > 2f) {
            movementController.Move(1.0f);
        } else if (distanceToPlayer < -2f) {
            movementController.Move(-1.0f);
        } else {
            // get hopping when at the spot
            movementController.Jump();
        }
    }
}
