using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerHeadVulnerable : Vulnerable
{
    public override bool testAttack(Vector2 point) {
        return point.y > transform.position.y;
    }
}
