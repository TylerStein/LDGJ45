using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public StateType type;
    protected GameObject gameObject;

    public BaseState(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public abstract StateType Update();
}