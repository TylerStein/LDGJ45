using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StickSide
{
    LEFT = 0,
    CEILING = 1,
    RIGHT = 2
}

[CreateAssetMenu(fileName="New Wall Movement Settings", menuName="LDGJ45/WallMovementSettings", order=3)]
public class WallMovementSettings : ScriptableObject
{
    [Tooltip("Layer of the surface collider to detect edges")]
    [SerializeField] public LayerMask surfaceLayerMask;

    [Tooltip("Movement velocity along surface")]
    [SerializeField] public float surfaceMoveVelocity = 10.0f;

    [Tooltip("Dampen ramp to reach surface move speed")]
    [SerializeField] public float surfaceMoveSmoothing = 0.05f;

    [Tooltip("Dampen ramp to stopping on the surface")]
    [SerializeField] public float surfaceStopSmoothing = 0.1f;

    [Tooltip("Gravity pull to surface")]
    [SerializeField] public float surfacePull = 0.1f;

    [Header("Contact Distances")]

    [Tooltip("How close should the ground be below to consider touching (meters)")]
    [SerializeField] public float minGroundDistance = 0.01f;

    [Tooltip("How close should the ceiling be above to consider touching (meters")]
    [SerializeField] public float minCeilingDistance = 0.01f;

    [Tooltip("How close should a wall be beside the player to consider touching (meters)")]
    [SerializeField] public float minWallDistance = 0.01f;
}
