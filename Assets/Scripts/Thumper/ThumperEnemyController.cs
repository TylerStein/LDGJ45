using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThumperMovementController))]
[RequireComponent(typeof(AwarenessProvider))]
public class ThumperEnemyController : EnemyController
{
    [SerializeField] public ThumperMovementController movementController;
    [SerializeField] public GameObject attackCollider;
    [SerializeField] public ScrapSpawner scrapSpawner;
    [SerializeField] public Enemy_Sound_Controller soundController;

    public void Awake()
    {
        if (!movementController) movementController = GetComponent<ThumperMovementController>();
        if (!awarenessProvider) awarenessProvider = GetComponent<AwarenessProvider>();
        GetComponentInChildren<ThumperVulnerable>().vulnerableTo = vulnerableTo;

        Dictionary<StateType, BaseState> states = new Dictionary<StateType, BaseState>()
        {
            {StateType.Patrol, new ThumperPatrolState(this) },
            {StateType.Attack, new ThumperAttackState(this) }
        };
        stateMachine.SetStates(states);
    }

    public override void ReceiveAttack(AttackType attackType, Collider2D collider, Vector2 point)
    {
        movementController.ClearVelocity();
        awarenessProvider.Player.OnSuccessfulAttack(attackType, this, point);
       // animator.SetTrigger("Damage");

        scrapSpawner.transform.parent = null;
        scrapSpawner.Spawn();

        soundController.PlayDie();

        GameStateController.Instance.OnEnemyDie(this);
        movementController.rigidbody.simulated = false;
        movementController.collider.enabled = false;
        movementController.enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1.0f);
        enabled = false;
    }

    public override void GiveAttack()
    {
        movementController.ClearVelocity();
        Debug.DrawLine(transform.position, awarenessProvider.Player.transform.position, Color.red);
        awarenessProvider.Player.ReceiveAttack(this);
        vfxController.SpawnSwipeVFX(transform.position, Vector3.zero, Color.white);
        soundController.PlayAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateController.Instance.IsPlaying == false) return;
        stateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if (collision.gameObject.tag == "Player")
        {
            GiveAttack();
        }
    }
}