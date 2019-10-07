using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloaterAttackState : BaseState
{
    protected FloatBlastEnemyController controller;
    protected AwarenessProvider awarenessProvider;
    protected FloaterMovementController movementController;
    [SerializeField] private float slamHoldTick = 0f;

    public FloaterAttackState(FloatBlastEnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Attack;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<FloaterMovementController>();
    }

    public override void Activate()
    {
        controller.attackController.Activate();
        movementController.ClearVelocityX();
        return;
    }

    public override StateType Update()
    {
        if(controller.attackController.State == AOEState.IDLE)
        {
            return StateType.Patrol;
        }
        return type;
    }
}