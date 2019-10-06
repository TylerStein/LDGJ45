using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputProvider : MonoBehaviour
{

    public float Horizontal { get; private set; } = 0f;
    public float Vertical { get; private set; } = 0f;
    public bool JumpDown { get; private set; } = false;
    public bool Attack1 { get; private set; } = false;
    public bool Attack2 { get; private set; } = false;
    public bool Attack3 { get; private set; } = false;


    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private string attack1 = "Fire1";
    [SerializeField] private string attack2 = "Fire2";
    [SerializeField] private string attack3 = "Fire3";

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxis(horizontalAxis);
        Vertical = Input.GetAxis(verticalAxis);
        JumpDown = Input.GetButtonDown(jumpButton);
        Attack1 = Input.GetButtonDown(attack1);
        Attack2 = Input.GetButtonDown(attack2);
        Attack3 = Input.GetButtonDown(attack3);
    }
}
