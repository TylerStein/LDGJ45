using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThumperMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class ThumperEnemyController : EnemyController
{
    [SerializeField] private bool didAttack = false;
    [SerializeField] private bool isSlamming = false;
    [SerializeField] private float slamTick = 0f;
    [SerializeField] private float slamHoldTick = 0f;

    [SerializeField] public ThumperMovementController movementController;
    [SerializeField] public AwarenessProvider awarenessProvider;

    [SerializeField] private int moveDirection = 1;


    public void Awake() {
        if (!movementController) movementController = GetComponent<ThumperMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        isSlamming = false;
        movementController.ClearVelocity();
        Vector2 inDirection = ((Vector2)transform.position - point).normalized;
        movementController.AddForce(inDirection * 5.0f);
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);

    }

    public override void GiveAttack() {
        movementController.ClearVelocity();
        Debug.DrawLine(transform.position, awarenessProvider.Player.transform.position, Color.red);
        awarenessProvider.Player.ReceiveAttack(this);
    }

    // Update is called once per frame
    void Update() {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Sign(distanceToPlayer) != moveDirection) {
            moveDirection = moveDirection * -1;
        }

        if (distanceToPlayer > movementController.thumperSettings.slamMaxHorizontalDistance) {
            movementController.Move(1f);
        } else if (distanceToPlayer < -movementController.thumperSettings.slamMaxHorizontalDistance) {
            movementController.Move(-1f);
        } else if (movementController.IsAtHoverHeight && !isSlamming && slamTick == movementController.thumperSettings.slamDelay) {
            isSlamming = true;
            didAttack = false;
            slamTick = 0f;
            slamHoldTick = 0f;
            movementController.ClearVelocityX();
        }

        if (isSlamming) {
            if (slamHoldTick < movementController.thumperSettings.slamHoldDuration) {
              //  movementController.AddForce(Vector2.down * slamForce);

                float t = slamHoldTick / movementController.thumperSettings.slamHoldDuration;
                movementController.AddForce(Vector2.down * movementController.thumperSettings.slamForce * t);

                if (movementController.IsBlocked == true) {
                    if (movementController.IsGrounded == false) {
                        // stopped on something other than a ground piece
                        isSlamming = false;
                    }
                }

                slamHoldTick += Time.deltaTime;
                if (slamHoldTick > movementController.thumperSettings.slamHoldDuration) slamHoldTick = movementController.thumperSettings.slamHoldDuration;
            } else {
                isSlamming = false;
            }
        } else if (slamTick < movementController.thumperSettings.slamDelay && movementController.IsAtHoverHeight) {
            slamTick += Time.deltaTime;
            if (slamTick >= movementController.thumperSettings.slamDelay) {
                slamTick = movementController.thumperSettings.slamDelay;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Player") {
            // is beneath
            if (collision.contacts[0].point.y < transform.position.y && movementController.Velocity.y < 0.01f) {
                if (isSlamming && !didAttack) {
                    didAttack = true;
                    GiveAttack();
                }
            }
        }
    }
}
