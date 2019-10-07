using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [SerializeField] public AwarenessProvider awarenessProvider;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Animator animator;
    public StateMachine stateMachine = new StateMachine();

    public abstract void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point);
    public abstract void GiveAttack();
}
