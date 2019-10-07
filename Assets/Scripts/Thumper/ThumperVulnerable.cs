using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumperVulnerable : Vulnerable
{
    public override bool testAttack(Vector2 point) {
        return true;
    }
}
