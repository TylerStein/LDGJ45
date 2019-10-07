using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public PlayerInputProvider inputProvider;
    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public PlayerAbilityController abilityController;
    [SerializeField] public PlayerUIController uiController;
    [SerializeField] public SpriteVFXController vfxController;
    [SerializeField] public CameraController cameraController;
    [SerializeField] public ScrapSpawner scrapSpawner;
    [SerializeField] public Player_Sound_controller soundController;

    public int health = 4;
    [SerializeField] public SpriteRenderer playerSprite;
    [SerializeField] public Animator animator;

    [SerializeField] private bool _lastHadSlam = false;
    [SerializeField] private bool _lastHadPunch = false;
    [SerializeField] private bool _hasDied = false;

    public void OnSuccessfulAttack(AttackType attackType, EnemyController enemy, Vector2 point) {
        Debug.Log("hit " + enemy.gameObject.name + " with attack " + attackType.ToString());
        if (attackType == AttackType.Jump || attackType == AttackType.Slam) {
            Vector2 inDirection = (point - (Vector2)transform.position).normalized;
            Vector2 inNormal = Vector2.up;
            Vector2 reflection = Vector2.Reflect(inDirection, inNormal);
            movementController.AddForce(reflection * 10.0f);
            vfxController.SpawnSwipeVFX(enemy.transform.position, Vector3.zero, Color.red);

        } else if (attackType == AttackType.Punch) {
            Vector2 inDirection = (point - (Vector2)transform.position).normalized;
            Vector2 inNormal = Vector2.up;
            Vector2 reflection = Vector2.Reflect(inDirection, inNormal);
            movementController.AddForce(reflection * 2.0f);
            vfxController.SpawnSwipeVFX(enemy.transform.position, Vector3.zero, Color.green);
            abilityController.hasPunch = true;

        }
        cameraController.Shake(0.02f, 0.01f, 0.25f);

        if (enemy is BasicEnemyController) abilityController.hasPunch = true;
        else if (enemy is ThumperEnemyController) abilityController.hasSlam = true;
    }

    public void ReceiveAttack(EnemyController enemy)
    {
        cameraController.Shake(0.02f, 0.01f, 0.25f);
        vfxController.SpawnSwipeVFX(transform.position, Vector3.zero, Color.white);
        health--;
        uiController.SetHealth(health);
        if (health <= 0) {
            Die();
        } else {
            animator.SetTrigger("Damage");
            soundController.PlayTakeDamage();
        }
    }

    public void Die() {
        if (_hasDied) return;

        _hasDied = true;
        health = 0;
        uiController.SetHealth(health);
        soundController.PlayDie();

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        GameStateController.Instance.OnPlayerDie();

        animator.enabled = false;

        playerSprite.enabled = false;
        movementController.collider.enabled = false;
        movementController.rigidbody.simulated = false;
        movementController.enabled = false;
        enabled = false;
    }

    // Start is called before the first frame update
    void Start() {
        uiController.SetHealth(health);
        _lastHadPunch = abilityController.hasPunch;
        _lastHadSlam = abilityController.hasSlam;

        animator.SetBool("HasSlam", abilityController.hasSlam);
        animator.SetBool("HasPunch", abilityController.hasPunch);

        _hasDied = false;
    }

    // Update is called once per frame
    void Update() {
        if (GameStateController.Instance.IsPlaying == false) return;

        movementController.Move(inputProvider.Horizontal);

        if (inputProvider.JumpDown) {
            if (movementController.Jump()) {
                soundController.PlayJump();
            }
        }

        if (inputProvider.Attack1) {
            if (abilityController.Punch()) {
                animator.SetTrigger("Punch");
                soundController.PlayPunch();
            }
        } else if (inputProvider.Attack2) {
            if (abilityController.Slam()) {
                animator.SetTrigger("Slam");
                soundController.PlayPunch();
            }
        }

        //animation
        animator.SetFloat("VelocityX", Mathf.Abs(movementController.Velocity.x));
        animator.SetFloat("VelocityY", movementController.Velocity.y);
        animator.SetBool("IsBlocked", movementController.IsBlocked);

        if (_lastHadPunch != abilityController.hasPunch) {
            _lastHadPunch = abilityController.hasPunch;
            animator.SetBool("HasPunch", abilityController.hasPunch);
            animator.SetTrigger("ChangeAbilities");
        }

        if (_lastHadSlam != abilityController.hasSlam) {
            _lastHadSlam = abilityController.hasPunch;
            animator.SetBool("HasSlam", abilityController.hasSlam);
            animator.SetTrigger("ChangeAbilities");
        }

        if (movementController.LastDirection > 0) playerSprite.flipX = true;
        else playerSprite.flipX = false;

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //if (collision.collider.gameObject.tag == "Enemy") {
        //    Vulnerable consumer = collision.collider.gameObject.GetComponent<Vulnerable>();
        //    if (consumer) {
        //        if (!movementController.IsGrounded) consumer.RecieveAttack(AttackType.Jump, collision.contacts[0].point);
        //    }
        //}
    }
}