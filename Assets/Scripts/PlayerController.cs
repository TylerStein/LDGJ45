﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public PlayerInputProvider inputProvider;
    [SerializeField] public GroundMovementController movementController; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementController.Move(inputProvider.Horizontal);

        if (inputProvider.JumpDown) movementController.Jump();
    }
}
