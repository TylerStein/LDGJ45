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
    [SerializeField] public FloaterMovementControllerr movementController;
    [SerializeField] public Animator animator;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public AOEAttackController attackController;

    [SerializeField] public FloatBlastState state;
    [SerializeField] public int moveDirection = 1;


    public void Awake() {
        if (!movementController) movementController = GetComponent<FloaterMovementControllerr>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();
    }

    public override void GiveAttack() {
        attackController.transform.position = transform.position;
        attackController.Activate();
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
    }

    private void Update() {
        if (GameStateController.Instance.IsPlaying == false) return;

        if (state == FloatBlastState.MOVING) {
            movementController.Move(moveDirection);
            if (movementController.TouchingWallDirection == moveDirection) {
                // hit a wall, do the thing
                state = FloatBlastState.ATTACK;
            }
        } else if (state == FloatBlastState.ATTACK) {
            // begin attack and wait
            attackController.Activate();
            state = FloatBlastState.WAITING;
        } else if (state == FloatBlastState.WAITING) {
            if (attackController.State == AOEState.IDLE) {
                // attack complete
                state = FloatBlastState.MOVING;
                moveDirection = -moveDirection;
            }
        }
    }
}
