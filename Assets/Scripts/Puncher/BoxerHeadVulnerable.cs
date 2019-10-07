using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerHeadVulnerable : Vulnerable
{
    public override bool testAttack(Collider2D source, RaycastHit2D hit) {
        return source.transform.position.y > transform.position.y;
    }
}
