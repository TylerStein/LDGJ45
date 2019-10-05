using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class BasicEnemyController : MonoBehaviour
{
    [SerializeField] public LayerMask wallLayerMask;

    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public AwarenessProvider awarenessProvider;

    [SerializeField] private int moveDirection = 1;
    [SerializeField] private float moveToXTarget = 0f;

    // Update is called once per frame
    void Update()
    {
        // BEHAVIOUR:
        // - move away from player to within 2m of the nearest wall in the direction away from the player


        // 1. Determine if player is on correct side
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
