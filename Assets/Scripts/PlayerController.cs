using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public PlayerInputProvider inputProvider;
    [SerializeField] public GroundMovementController movementController;

    public void OnSuccessfulAttack(AttackType attackType, EnemyController enemy, Vector2 point) {
        if (attackType == AttackType.Jump) {
            Vector2 inDirection = (point - (Vector2)transform.position).normalized;
            Vector2 inNormal = Vector2.up;
            Vector2 reflection = Vector2.Reflect(inDirection, inNormal);
            movementController.AddForce(reflection * 10.0f);
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        movementController.Move(inputProvider.Horizontal);

        if (inputProvider.JumpDown) movementController.Jump();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //GameObject other = collision.gameObject;
        //if (other.CompareTag("Enemy")) {
        //    if (!movementController.IsGrounded) {
        //        if (other.GetComponent<Vulnerable>().RecieveAttack(AttackType.Jump)) {
        //            other.GetComponent<EnemyController>().ReceiveAttack(AttackType.Jump, collision.collider, collision.contacts[0].point);
        //        }
        //    }
        //    if (inputProvider.JumpDown) movementController.Jump();
        //}

        if (collision.collider.gameObject.tag == "Enemy") {
            Vulnerable consumer = collision.collider.gameObject.GetComponent<Vulnerable>();
            if (consumer) {
                if (!movementController.IsGrounded) consumer.RecieveAttack(AttackType.Jump, collision.contacts[0].point);
            }
        }
    }
}