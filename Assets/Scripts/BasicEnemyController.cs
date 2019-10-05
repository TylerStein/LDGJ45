using System.Collections;
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
    [SerializeField] private float attackCooldown = 1f;
    private float attackTimer = 0;

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
    }

    public override void GiveAttack()
    {
        Vector2 collideBoxOrigin = new Vector2(moveDirection * 0.4f, 0);
        Collider2D collider = Physics2D.OverlapBox(collideBoxOrigin, new Vector2(0.2f, 0.2f), 0f, LayerMask.NameToLayer("Player"));

        PlayerController p = collider.gameObject.GetComponent<PlayerController>();
        if (p)
        {
            p.ReceiveAttack();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Sign(distanceToPlayer) != moveDirection) {
            moveDirection = moveDirection * -1;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0)
                attackTimer = 0;
        }
  
        if (distanceToPlayer > 2f) {
            movementController.Move(1.0f);
        } else if (distanceToPlayer < -2f) {
            movementController.Move(-1.0f);
        } else if(attackTimer == 0){
            GiveAttack();
            attackTimer = attackCooldown;
        }
    }
}
