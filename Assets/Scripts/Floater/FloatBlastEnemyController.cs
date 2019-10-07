using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloatBlastState
{
    MOVING = 0,
    ATTACK = 1,
    WAITING = 2,
}

public class FloatBlastEnemyController : EnemyController
{
    [SerializeField] public FloaterMovementController movementController;
    [SerializeField] public AOEAttackController attackController;

    [SerializeField] public FloatBlastState state;
    [SerializeField] public int moveDirection = 1;


    public void Awake() {
        if (!movementController) movementController = GetComponent<FloaterMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();

        Dictionary<StateType, BaseState> states = new Dictionary<StateType, BaseState>()
        {
            {StateType.Patrol, new FloaterPatrolState(this) },
            {StateType.Attack, new FloaterAttackState(this) }
        };
        stateMachine.SetStates(states);
    }

    public override void GiveAttack() {
        attackController.transform.position = transform.position;
        attackController.Activate();
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
        GameStateController.Instance.OnEnemyDie(this);
    }

    private void Update() {
        if (GameStateController.Instance.IsPlaying == false) return;

        stateMachine.Update();
    }
}
