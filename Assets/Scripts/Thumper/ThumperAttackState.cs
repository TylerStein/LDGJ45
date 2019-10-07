using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperAttackState : BaseState
{
    protected ThumperEnemyController controller;
    protected AwarenessProvider awarenessProvider;
    protected ThumperMovementController movementController;
    [SerializeField] private float slamHoldTick = 0f;

    public ThumperAttackState(ThumperEnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Attack;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
        movementController = gameObject.GetComponent<ThumperMovementController>();
    }

    public override void Activate()
    {
        slamHoldTick = 0f;
        controller.attackCollider.SetActive(true);
        movementController.ClearVelocityX();
    }

    public override StateType Update()
    {
        if (slamHoldTick < movementController.thumperSettings.slamHoldDuration)
        {
            //  movementController.AddForce(Vector2.down * slamForce);

            float t = slamHoldTick / movementController.thumperSettings.slamHoldDuration;
            movementController.AddForce(Vector2.down * movementController.thumperSettings.slamForce * t);

            if (movementController.IsBlocked == true)
            {
                if (movementController.IsGrounded == false)
                {
                    // stopped on something other than a ground piece
                    controller.attackCollider.SetActive(false);
                    return StateType.Patrol;
                }
            }

            slamHoldTick += Time.deltaTime;
            if (slamHoldTick > movementController.thumperSettings.slamHoldDuration)
            {
                slamHoldTick = movementController.thumperSettings.slamHoldDuration;
            }
        }
        else
        { 
            controller.attackCollider.SetActive(false);
            return StateType.Patrol;
        }
        return type;
    }
}