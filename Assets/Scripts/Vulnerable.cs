using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   public  enum AttackType
    {
        Jump = 0,
        Punch
    }

public class Vulnerable : MonoBehaviour
{
    public AttackType vulnerableTo;

    public bool RecieveAttack(AttackType incomingType)
    {
        return incomingType == vulnerableTo;
    }
}
