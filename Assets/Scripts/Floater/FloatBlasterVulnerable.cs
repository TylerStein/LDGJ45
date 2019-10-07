using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBlasterVulnerable : Vulnerable
{
    public override bool testAttack(Collider2D source, RaycastHit2D hit) {
        return true;
    }
}
