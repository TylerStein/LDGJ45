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
    [SerializeField] public Enemy_Sound_Controller soundController;

    [SerializeField] public ScrapSpawner scrapSpawner;

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
        animator.SetBool("Attacking", true);
        attackController.Activate();
        state = FloatBlastState.WAITING;
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);

        scrapSpawner.transform.parent = null;

        scrapSpawner.Spawn();
        soundController.PlayDie();

        GameStateController.Instance.OnEnemyDie(this);
        movementController.rigidbody.simulated = false;
        movementController.collider.enabled = false;
        movementController.enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1.0f);
        enabled = false;
    }

    private void Update() {
        if (GameStateController.Instance.IsPlaying == false) return;

        stateMachine.Update();
        if (state == FloatBlastState.MOVING) {
            movementController.Move(moveDirection);
            spriteRenderer.flipX = moveDirection < 0;
            if (movementController.TouchingWallDirection == moveDirection) {
                // hit a wall, do the thing
                state = FloatBlastState.ATTACK;
            }
        } else if (state == FloatBlastState.ATTACK) {
            // begin attack and wait
            GiveAttack();
        } else if (state == FloatBlastState.WAITING) {
            if (attackController.State == AOEState.IDLE) {
                // attack complete
                state = FloatBlastState.MOVING;
                animator.SetBool("Attacking", false);
                moveDirection = -moveDirection;
            }
        }
    }
}
