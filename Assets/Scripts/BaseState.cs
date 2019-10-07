using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public StateType type;
    public GameObject StateGameObject { get { return gameObject; } }
    protected GameObject gameObject;

    public BaseState(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
    public abstract void Activate();
    public abstract StateType Update();
}