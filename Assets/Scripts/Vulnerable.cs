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
    [SerializeField] public EnemyController owner;
    [SerializeField] public new Collider2D collider;
    [SerializeField] public AttackType[] vulnerableTo = { AttackType.Jump };

    public abstract bool testAttack(Collider2D source, RaycastHit2D hit);

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

    public bool RecieveAttack(AttackType incomingType, Collider2D source, RaycastHit2D hit)
    {
        bool inList = false;
        for (int i = 0; i < vulnerableTo.Length; i++) {
            if (incomingType == vulnerableTo[i]) {
                inList = true;
                break;
            }
        }
        if (inList && testAttack(source, hit)) {
            owner.ReceiveAttack(incomingType, collider, hit.point);
            return true;
        }
        return false;
    }
}
