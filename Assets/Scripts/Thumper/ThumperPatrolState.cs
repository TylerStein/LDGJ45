using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperPatrolState : BaseState
{
    protected AwarenessProvider awarenessProvider;
    protected ThumperMovementController movementController;
    [SerializeField] private int moveDirection = 1;
    [SerializeField] private Vector3 origPosition;
    [SerializeField] private float patrolRange = 2.5f;
    [SerializeField] private float slamTick = 0f;

    public ThumperPatrolState(EnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Patrol;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<ThumperMovementController>();
        origPosition = movementController.transform.position;
    }

    public override void Activate()
    {
        return;
    }

    public override StateType Update()
    {
        if (slamTick < movementController.thumperSettings.slamDelay && movementController.IsAtHoverHeight)
        {
            slamTick += Time.deltaTime;
            if (slamTick >= movementController.thumperSettings.slamDelay)
            {
                slamTick = movementController.thumperSettings.slamDelay;
            }
        }

        float distanceToPlayer = awarenessProvider.GetHorizontalDistanceToPlayer();
        if (Mathf.Abs(distanceToPlayer) < movementController.thumperSettings.slamMaxHorizontalDistance
            && movementController.IsAtHoverHeight && slamTick == movementController.thumperSettings.slamDelay)
        {
            slamTick = 0f;
            return StateType.Attack;
        }

        float distanceToOrigin = (origPosition - movementController.transform.position).x;

        if (Mathf.Abs(distanceToOrigin) > patrolRange && Mathf.Sign(distanceToOrigin) != moveDirection)
        {
            moveDirection *= -1;
        }
        movementController.Move(moveDirection);
        return type;
    }
}