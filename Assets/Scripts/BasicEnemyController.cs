using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class BasicEnemyController : EnemyController
{
    [SerializeField] public LayerMask wallLayerMask;
    [SerializeField] public LayerMask attackLayerMask;

    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public AwarenessProvider awarenessProvider;
    [SerializeField] public Animator animator;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public ScrapSpawner scrapSpawner;

    [SerializeField] private float punchMoveForce = 10.0f;
    [SerializeField] private float punchDistance = 0.7f;
    [SerializeField] private float stopDistance = 0.9f;
    [SerializeField] private int moveDirection = 1;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackTimer = 0;

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
        animator.SetTrigger("Damage");

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        Destroy(gameObject, 0.1f);
    }

    public override void GiveAttack()
    {
        Vector2 collideBoxOrigin = new Vector2(moveDirection * punchDistance + transform.position.x, transform.position.y);
        Collider2D collider = Physics2D.OverlapBox(collideBoxOrigin, new Vector2(0.5f, 0.2f), 0f, attackLayerMask);

        movementController.AddForce(Vector2.right * moveDirection * punchMoveForce);
        animator.SetTrigger("Attack");

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

        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Sign(distanceToPlayer) != moveDirection) {
            moveDirection = moveDirection * -1;
        }

        animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        spriteRenderer.flipX = moveDirection > 0;

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0)
                attackTimer = 0;
        }

        if (distanceToPlayer > stopDistance) {
            movementController.Move(1.0f);
        } else if (distanceToPlayer < -stopDistance) {
            movementController.Move(-1.0f);
        } else if(attackTimer == 0) {
            attackTimer = attackCooldown;
            GiveAttack();
        }
    }
}
