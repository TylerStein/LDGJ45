using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WallMovementController))]
public class ClimberEnemyController : EnemyController
{
    [SerializeField] public WallMovementController movementController;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Transform weaponTransform;

    [SerializeField] public float moveTick = 0f;
    [SerializeField] public float directionTime = 1f;
    [SerializeField] public float direction = 1f;

    public override void GiveAttack() {
    //    throw new System.NotImplementedException();
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
    //    throw new System.NotImplementedException();
    }

    private void Start() {
        if (!movementController) movementController = GetComponent<WallMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.flipY = (movementController.stickSide == StickSide.CEILING);
        spriteRenderer.flipX = (movementController.stickSide == StickSide.RIGHT);



        if (movementController.IsOnWall) {
            moveTick += Time.deltaTime;
            if (moveTick > directionTime) {
                moveTick = 0f;
                direction *= -1;
            }
        }

        movementController.Move(direction);
    }
}
