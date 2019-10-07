﻿using System.Collections;
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

    public int health = 4;
    [SerializeField] public SpriteRenderer playerSprite;
    [SerializeField] public Animator animator;

    public void OnSuccessfulAttack(AttackType attackType, EnemyController enemy, Vector2 point) {
        Debug.Log("hit " + enemy.gameObject.name + " with attack " + attackType.ToString());
        if (attackType == AttackType.Jump) {
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
        }
        cameraController.Shake(0.02f, 0.01f, 0.25f);
    }

    public void ReceiveAttack(EnemyController enemy)
    {
        cameraController.Shake(0.02f, 0.01f, 0.25f);
        Debug.Log("Got hit by " + enemy.gameObject.name);
        vfxController.SpawnSwipeVFX(transform.position, Vector3.zero, Color.white);
        health--;
        uiController.SetHealth(health);
        if (health <= 0) {
            Die();
        } else {
            animator.SetTrigger("Damage");
        }
    }

    public void Die() {
        health = 0;
        uiController.SetHealth(health);
        animator.SetTrigger("Die");

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        GameStateController.Instance.OnPlayerDie();
        animator.enabled = false;
        playerSprite.enabled = false;
        enabled = false;
    }

    // Start is called before the first frame update
    void Start() {
        uiController.SetHealth(health);
    }

    // Update is called once per frame
    void Update() {
        if (GameStateController.Instance.IsPlaying == false) return;

        movementController.Move(inputProvider.Horizontal);

        if (inputProvider.JumpDown) movementController.Jump();

        if (inputProvider.Attack1) {
            if (abilityController.Punch()) {
                animator.SetTrigger("Punch");
            }
        } else if (inputProvider.Attack2) {
            if (abilityController.Slam()) {
                animator.SetTrigger("Slam");
            }
        }

        //animation
        animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        animator.SetBool("Is_Grounded?", movementController.IsGrounded);

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