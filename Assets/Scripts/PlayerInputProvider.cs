using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputProvider : MonoBehaviour
{

    public float Horizontal { get; private set; } = 0f;
    public float Vertical { get; private set; } = 0f;
    public bool JumpDown { get; private set; } = false;
    public bool BasicAttackDown { get; private set; } = false;

    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private string basicAttackButton = "Fire1";

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxis(horizontalAxis);
        Vertical = Input.GetAxis(verticalAxis);
        JumpDown = Input.GetButtonDown(jumpButton);
        BasicAttackDown = Input.GetButtonDown(basicAttackButton);
    }
}
