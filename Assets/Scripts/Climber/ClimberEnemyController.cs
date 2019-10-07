using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WallMovementController))]
public class ClimberEnemyController : EnemyController
{
    [SerializeField] public WallMovementController movementController;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Transform weaponTransform;

    [SerializeField] public Transform laserSource;
    [SerializeField] public SpriteRenderer laserSprite;

    [SerializeField] public float shootDelay = 3.0f;
    [SerializeField] private float _shootTick = 0f;

    [SerializeField] public float laserFadeTime = 0.5f;
    [SerializeField] public float laserFadeTick = 0f;

    [SerializeField] public float moveTick = 0f;
    [SerializeField] public float directionTime = 1f;
    [SerializeField] public float direction = 1f;

    public override void GiveAttack() {
        Debug.DrawRay(weaponTransform.position, weaponTransform.up * -1 * 100, Color.green, 5.0f, false);
        float sizeX = 20f;
        float sizeY = 0.8f;
        laserFadeTick = 0f;
        laserSprite.color = new Color(1, 1, 1, 1);
        laserSprite.drawMode = SpriteDrawMode.Tiled;
        laserSprite.size = new Vector2(sizeX, sizeY);
        laserSource.position = transform.position;
        laserSprite.transform.localPosition = new Vector3(sizeX / 2, 0, 0);
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
    }

    private void Start() {
        if (!movementController) movementController = GetComponent<WallMovementController>();
        laserFadeTick = laserFadeTime;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.flipY = (movementController.stickSide == StickSide.CEILING);
        spriteRenderer.flipX = (movementController.stickSide == StickSide.RIGHT);

        float rotation = 0;
        if (movementController.stickSide == StickSide.RIGHT) rotation = -90f;
        else if (movementController.stickSide == StickSide.LEFT) rotation = 90f;
        weaponTransform.localRotation = Quaternion.Euler(0, 0, rotation);

        if (movementController.IsOnWall) {
            moveTick += Time.deltaTime;
            if (moveTick > directionTime) {
                moveTick = 0f;
                direction *= -1;
            }
        }

        if (_shootTick < shootDelay) {
            _shootTick += Time.deltaTime;
            if (_shootTick >= shootDelay) {
                _shootTick = 0f;
                GiveAttack();
            }
        }

        if (laserFadeTick < laserFadeTime) {
            laserFadeTick += Time.deltaTime;
            float progress = laserFadeTick / laserFadeTime;
            laserSprite.color = new Color(1, 1, 1, 1 - progress);
        }

        movementController.Move(direction);
    }
}
