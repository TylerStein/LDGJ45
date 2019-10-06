using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Ground Movement Settings", menuName = "LDGJ45/MovementSettings", order=0)]
public class GroundMovementSettings : ScriptableObject
{
    [Header("Ground Movement")]

    [Tooltip("Movement Velocity on Ground (meters/s)")]
    [SerializeField] public float groundMoveVelocity = 10.0f;

    [Tooltip("Dampen ramp to reach ground move speed")]
    [SerializeField] public float groundMoveSmoothing = 0.05f;

    [Tooltip("Dampen ramp to stopping horizontally on the ground")]
    [SerializeField] public float groundStopSmoothing = 0.1f;

    [Tooltip("Allow the player to stick to walls and jump off of them")]
    [SerializeField] public bool enableWallJump = false;

    [Header("Air Movement")]

    [Tooltip("Jump Force")]
    [SerializeField] public float jumpForce = 600.0f;

    [Tooltip("Allow Air Movement")]
    [SerializeField] public bool canMoveInAir = true;

    [Tooltip("Dampen ramp to reach air move speed")]
    [SerializeField] public float airMoveSmoothing = 0.05f;

    [Tooltip("Allow dampening of horizontal air velocity")]
    [SerializeField] public bool dampenAirMovement = false;

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
