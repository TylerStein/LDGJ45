using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Floater Movement Settings", menuName ="LDGJ45/Floater Movement Settings", order = 4)]
public class FloaterMovementSettings : ScriptableObject
{
    [Header("Hovering")]
    [SerializeField] public float hoverHeight = 5.0f;
    [SerializeField] public float hoverErrorMargin = 0.5f;
    [SerializeField] public float hoverHeightSpeed = 5.0f;
    [SerializeField] public float hoverHeightSmoothing = 0.4f;

    [Header("Air Movement")]

    [Tooltip("Dampen ramp to reach air move speed")]
    [SerializeField] public float airMoveSmoothing = 0.05f;

    [Tooltip("Dampen ramp to stopping horizontally in the air")]
    [SerializeField] public float airStopSmoothing = 0.07f;

    [Tooltip("Movement Velocity in Air (meters/s)")]
    [SerializeField] public float airMoveVelocity = 2.0f;

    [Header("Contact Distances")]

    [Tooltip("How close should the ground be below to consider touching (meters)")]
    [SerializeField] public float minGroundDistance = 0.01f;

    [Tooltip("How close should the ceiling be above to consider touching (meters")]
    [SerializeField] public float minCeilingDistance = 0.01f;

    [Tooltip("How close should a wall be beside the player to consider touching (meters)")]
    [SerializeField] public float minWallDistance = 0.01f;


    [Header("Contact Layers")]

    [Tooltip("Define Ground layer mask for contacts to count as IsGrounded")]
    [SerializeField] public LayerMask groundLayer;

    [Tooltip("Define Ceiling layer mask for contacts to count as IsTouchingCeiling")]
    [SerializeField] public LayerMask ceilingLayer;

    [Tooltip("Define Ceiling layer mask for contacts to count as IsTouchingWall")]
    [SerializeField] public LayerMask wallLayer;

}
