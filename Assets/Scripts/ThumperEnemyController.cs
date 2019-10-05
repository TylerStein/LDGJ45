using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThumperMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class ThumperEnemyController : EnemyController
{
    [SerializeField] public float thumperDelay = 1.5f;
    [SerializeField] private float thumperTick = 0f;

    [SerializeField] public LayerMask wallLayerMask;

    [SerializeField] public ThumperMovementController movementController;
    [SerializeField] public AwarenessProvider awarenessProvider;

    [SerializeField] private int moveDirection = 1;


    public void Awake() {
        if (!movementController) movementController = GetComponent<ThumperMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        Debug.Log("Thumper Receive Attack");
    }

    public override void GiveAttack() {
        movementController.ClearVelocity();
        Debug.Log("Thumper Give Attack");
    }

    // Update is called once per frame
    void Update() {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Sign(distanceToPlayer) != moveDirection) {
            moveDirection = moveDirection * -1;
        }

        if (distanceToPlayer > 2f) {
            movementController.Move(1.0f);
        } else if (distanceToPlayer < -2f) {
            movementController.Move(-1.0f);
        } else if (movementController.IsAtHoverHeight && thumperTick == thumperDelay) {
            // get slamming at this point
            movementController.Slam();
            thumperTick = 0f;
        }

        if (thumperTick < thumperDelay && movementController.IsAtHoverHeight) {
            thumperTick += Time.deltaTime;
            if (thumperTick >= thumperDelay) thumperTick = thumperDelay;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            // is beneath
            if (collision.contacts[0].point.y < transform.position.y && movementController.Velocity.y < 0.01f) {
                GiveAttack();
            }
        }
    }
}
