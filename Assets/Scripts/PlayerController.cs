using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public PlayerInputProvider inputProvider;
    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public SpriteRenderer playerSprite;
    [SerializeField] public Animator animator;

    public void OnSuccessfulAttack(AttackType attackType, EnemyController enemy, Vector2 point) {
        if (attackType == AttackType.Jump) {
            Vector2 inDirection = (point - (Vector2)transform.position).normalized;
            Vector2 inNormal = Vector2.up;
            Vector2 reflection = Vector2.Reflect(inDirection, inNormal);
            movementController.AddForce(reflection * 10.0f);
        } else if (attackType == AttackType.Punch) {
            Vector2 inDirection = (point - (Vector2)transform.position).normalized;
            Vector2 inNormal = Vector2.up;
            Vector2 reflection = Vector2.Reflect(inDirection, inNormal);
            movementController.AddForce(reflection * 2.0f);
        }
    }

    public void ReceiveAttack()
    {
        Debug.Log("Got hit");
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        movementController.Move(inputProvider.Horizontal);

        if (inputProvider.JumpDown) movementController.Jump();

        //animation
        animator.SetFloat("Velocity", Mathf.Abs(movementController.Velocity.x));
        animator.SetBool("Is_Grounded?", movementController.IsGrounded);
        if (movementController.LastDirection > 0) playerSprite.flipX = true;
        else playerSprite.flipX = false;

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.tag == "Enemy") {
            Vulnerable consumer = collision.collider.gameObject.GetComponent<Vulnerable>();
            if (consumer) {
                if (!movementController.IsGrounded) consumer.RecieveAttack(AttackType.Jump, collision.contacts[0].point);
            }
        }
    }
}