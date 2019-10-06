using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] public LayerMask enemyLayerMask;

    [Header("Punch")]
    [SerializeField] public bool hasPunch = true;
    [SerializeField] private float punchDistance = 1.5f;
    [SerializeField] private float punchLungeForce = 0.5f;

    [Header("Slam")]
    [SerializeField] public bool hasSlam;

    [SerializeField] private Collider2D[] _overlaps = new Collider2D[4];
    [SerializeField] private RaycastHit2D[] _hits = new RaycastHit2D[4];

    public void Punch(PlayerController controller) {
        Vector2 direction = Vector2.right * controller.movementController.LastDirection;
        controller.movementController.AddForce(direction * punchLungeForce);
        int contacts = Physics2D.BoxCastNonAlloc(transform.position, Vector2.one * 0.75f, 0, direction, _hits, punchDistance, enemyLayerMask);
        for (int i = 0; i < contacts; i++) {
            if (_hits[i].collider.gameObject.tag == "Enemy") {
                Vulnerable consumer = _hits[i].collider.gameObject.GetComponent<Vulnerable>();
                if (consumer) {
                    consumer.RecieveAttack(AttackType.Punch, _hits[i].point);
                    Debug.DrawLine(transform.position, consumer.transform.position, Color.red, 2.0f);
                    return;
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position + ((Vector3)direction * punchDistance), Color.blue, 2.0f);
    }

    public void Slam(PlayerController controller) {

    }
}
