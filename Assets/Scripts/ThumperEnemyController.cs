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
        Debug.Log("Thumper.Ouch!");
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
}
