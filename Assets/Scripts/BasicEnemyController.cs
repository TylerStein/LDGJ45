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
    [SerializeField] private float attackTimer = 0;

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
        Debug.Log("Attacked by Player");
    }

    public override void GiveAttack()
    {
        Vector2 collideBoxOrigin = new Vector2(moveDirection * 0.5f + transform.position.x, transform.position.y);
        Collider2D collider = Physics2D.OverlapBox(collideBoxOrigin, new Vector2(0.5f, 0.2f), 0f, LayerMask.NameToLayer("Player"));

        if (collider == null)
            return;

        PlayerController p = collider.gameObject.GetComponent<PlayerController>();
        if (p)
        {
            p.ReceiveAttack();
            Debug.Log("Punched Player");
            Debug.DrawLine(transform.position, p.transform.position, Color.red);
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

        if (distanceToPlayer > 1.5f) {
            movementController.Move(1.0f);
        } else if (distanceToPlayer < -1.5f) {
            movementController.Move(-1.0f);
        } else if(attackTimer == 0) {
            attackTimer = attackCooldown;
            GiveAttack();
        }
    }
}
