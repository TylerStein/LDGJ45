using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChaseState : BaseState
{
    protected AwarenessProvider awarenessProvider;
    protected EnemyController controller;
    protected GroundMovementController movementController;
    [SerializeField] private int moveDirection = 1;
    [SerializeField] private float maxChaseRange = 5f;
    [SerializeField] private float attackRange = 1.5f;

    public GroundChaseState(EnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Chase;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<GroundMovementController>();
    }
    public override void Activate()
    {
        return;
    }

    public override StateType Update()
    {
        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();

        if (Mathf.Abs(distanceToPlayer) > maxChaseRange)
        {
            return StateType.Patrol;
        }
        else if (Mathf.Sign(distanceToPlayer) != moveDirection)
        {
            moveDirection *= -1;
        }
        else if (Mathf.Abs(distanceToPlayer) < attackRange)
        {
            return StateType.Attack;
        }
        movementController.Move(moveDirection);

        controller.animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        controller.spriteRenderer.flipX = moveDirection > 0;

        return type;
    }
}