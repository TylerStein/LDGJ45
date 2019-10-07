using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Patrol = 0,
    Chase,
    Attack
}

public class StateMachine
{
    private Dictionary<StateType, BaseState> availableStates;
    private BaseState currentState;

    public void SetStates(Dictionary<StateType, BaseState> states)
    {
        availableStates = states;
    }

    private void SwitchToNewState(StateType nextState)
    {
        if (availableStates.ContainsKey(nextState))
        {
            StateType oldState = currentState.type;
            currentState = availableStates[nextState];
            currentState.Activate();
            Debug.Log(currentState.StateGameObject.name + " state: " + oldState + " -> " + currentState.type);
        }
    }

    public void Update()
    {
        if(currentState == null)
        {
            currentState = availableStates.Values.First();
        }

        StateType nextState = currentState.Update();

        if(nextState != currentState.type)
        {
            SwitchToNewState(nextState);
        }
    }
}