using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public HealthBarView healthBarView;

    public void SetHealth(int health) {
        healthBarView.SetHealth(health);
    }
}
