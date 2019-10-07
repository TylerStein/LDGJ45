using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAttackState : BaseState
{
    protected EnemyController controller;
    protected AwarenessProvider awarenessProvider;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackTimer = 0;
    [SerializeField] private float attackRange = 1.6f;

    public GroundAttackState(EnemyController controller) : base(controller.gameObject)
    {
        type = StateType.Attack;
        this.controller = controller;
        awarenessProvider = controller.awarenessProvider;
    }
    public override void Activate()
    {
        return;
    }

    public override StateType Update()
    {
        if(attackTimer <= 0f)
        {
            if(Mathf.Abs(awarenessProvider.GetHorizontalDistanceToPlayer()) > attackRange)
            {
                return StateType.Chase;
            }
            attackTimer = attackCooldown;
            controller.GiveAttack();
        }
        attackTimer -= Time.deltaTime;

        return type;
    }
}