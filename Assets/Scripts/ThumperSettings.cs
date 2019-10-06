using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Thumper Settings", menuName="LDGJ45/ThumperSettings", order = 2)]
public class ThumperSettings : ScriptableObject
{
    [SerializeField] public float hoverErrorMargin = 0.5f;
    [SerializeField] public float hoverHeight = 4.0f;
    [SerializeField] public float hoverHeightSpeed = 5.0f;
    [SerializeField] public float hoverHeightSmoothing = 0.4f;
    [SerializeField] public float slamDelay = 1.5f;
    [SerializeField] public float slamHoldDuration = 0.75f;
    [SerializeField] public float slamForce = 10f;
    [SerializeField] public float slamMaxHorizontalDistance = 1.5f;
}
