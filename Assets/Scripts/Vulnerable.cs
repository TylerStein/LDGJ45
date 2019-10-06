using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   public  enum AttackType
    {
        Jump = 0,
        Punch = 1,
        Slam = 2
    }

[RequireComponent(typeof(Collider2D))]
public abstract class Vulnerable : MonoBehaviour
{
    [SerializeField] public AttackType vulnerableTo;
    [SerializeField] public EnemyController owner;
    [SerializeField] public new Collider2D collider;

    public abstract bool testAttack(Vector2 point);

    public void Awake() {
        if(!collider) collider = GetComponent<Collider2D>();
        if(!owner) {
            owner = GetComponentInParent<EnemyController>();
            if (!owner) {
                owner = GetComponent<EnemyController>();
                if (!owner) {
                    throw new UnityException("Vulnerable collider could not find an EnemyController parent! " + gameObject.name);
                }
            }
        }
    }

    public bool RecieveAttack(AttackType incomingType, Vector2 point)
    {
        if (incomingType == vulnerableTo && testAttack(point)) {
            owner.ReceiveAttack(incomingType, collider, point);
        }
        return incomingType == vulnerableTo;
    }
}
