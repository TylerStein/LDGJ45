using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPatrolState : BaseState
{
    protected AwarenessProvider awarenessProvider;
    protected EnemyController controller;
    protected GroundMovementController movementController;
    [SerializeField] private int moveDirection = 1;
    [SerializeField] private Vector3 origPosition;
    [SerializeField] private float chaseRange = 2f;
    [SerializeField] private float patrolRange = 2.5f;

    public GroundPatrolState(EnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Patrol;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<GroundMovementController>();
        origPosition = movementController.transform.position;
    }

    public override void Activate()
    {
        origPosition = movementController.transform.position;
    }

    public override StateType Update()
    {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Abs(distanceToPlayer) < chaseRange)
        {
            return StateType.Chase;
        }

        float distanceToOrigin = (origPosition - movementController.transform.position).x;

        if (Mathf.Abs(distanceToOrigin) > patrolRange && Mathf.Sign(distanceToOrigin) != moveDirection)
        {
            moveDirection *= -1;
        }
        movementController.Move(moveDirection);

        controller.animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        controller.spriteRenderer.flipX = moveDirection > 0;

        return type;
    }
}