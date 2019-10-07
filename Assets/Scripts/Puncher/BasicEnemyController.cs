using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class BasicEnemyController : EnemyController
{
    [SerializeField] public LayerMask wallLayerMask;
    [SerializeField] public LayerMask attackLayerMask;

    [SerializeField] public GroundMovementController movementController;
    [SerializeField] public ScrapSpawner scrapSpawner;
    [SerializeField] private float punchMoveForce = 10.0f;
    [SerializeField] private float punchDistance = 0.7f;
    
    private void Awake()
    {
        if (!movementController) movementController = GetComponent<GroundMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();

        Dictionary<StateType, BaseState> states = new Dictionary<StateType, BaseState>()
        {
            {StateType.Patrol, new GroundPatrolState(this) },
            {StateType.Chase, new GroundChaseState(this) },
            {StateType.Attack, new GroundAttackState(this) }
        };
        stateMachine.SetStates(states);
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point) {
        // Make the player bounce off the head
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
        animator.SetTrigger("Damage");

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        GameStateController.Instance.OnEnemyDie(this);
        Destroy(gameObject, 0.1f);
    }

    public override void GiveAttack()
    {
        Vector2 collideBoxOrigin = new Vector2(movementController.LastDirection * punchDistance + transform.position.x, transform.position.y);
        Collider2D collider = Physics2D.OverlapBox(collideBoxOrigin, new Vector2(0.5f, 0.2f), 0f, attackLayerMask);

        movementController.AddForce(Vector2.right * movementController.LastDirection * punchMoveForce);
        animator.SetTrigger("Attack");

        if (collider == null)
            return;

        PlayerController p = collider.gameObject.GetComponent<PlayerController>();
        if (p)
        {
            p.ReceiveAttack(this);
            Debug.DrawLine(transform.position, p.transform.position, Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.Instance.IsPlaying == false) return;

        stateMachine.Update();
    }
}
