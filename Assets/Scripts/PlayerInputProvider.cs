using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputProvider : MonoBehaviour
{

    public float Horizontal { get; private set; } = 0f;
    public float Vertical { get; private set; } = 0f;
    public bool JumpDown { get; private set; } = false;
    public bool PunchDown { get; private set; } = false;
    public bool SelectDown { get; private set; } = false;
    public bool StartDown { get; private set; } = false;
    public bool EscapeDown { get; private set; } = false;

    public bool AnyInput { get { return JumpDown || PunchDown; } }


    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string jumpButton = "Jump";
    [SerializeField] private string punchButton = "Punch";
    [SerializeField] private string selectButton = "Select";
    [SerializeField] private string startButton = "Start";
    [SerializeField] private string escapeButton = "Escape";

    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxis(horizontalAxis);
        Vertical = Input.GetAxis(verticalAxis);
        JumpDown = Input.GetButtonDown(jumpButton);
        PunchDown = Input.GetButtonDown(punchButton);
        SelectDown = Input.GetButtonDown(selectButton);
        StartDown = Input.GetButtonDown(startButton);
        EscapeDown = Input.GetButtonDown(escapeButton);
    }
}
