using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterPatrolState : BaseState
{
    protected AwarenessProvider awarenessProvider;
    protected EnemyController controller;
    protected FloaterMovementController movementController;
    [SerializeField] private int moveDirection = 1;
    [SerializeField] private Vector3 origPosition;
    [SerializeField] private float patrolRange = 2.5f;
    [SerializeField] private float slamTick = 0f;

    public FloaterPatrolState(EnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Patrol;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<FloaterMovementController>();
        origPosition = movementController.transform.position;
    }

    public override void Activate()
    {
        moveDirection *= -1;
        return;
    }

    public override StateType Update()
    {
        movementController.Move(moveDirection);

        if (movementController.TouchingWallDirection == moveDirection)
        {
            return StateType.Attack;
        }        

        controller.animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        controller.spriteRenderer.flipX = moveDirection > 0;

        return type;
    }
}