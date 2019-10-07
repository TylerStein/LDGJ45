using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBlasterVulnerable : Vulnerable
{
    public override bool testAttack(Vector2 point) {
        return true;
    }
}
