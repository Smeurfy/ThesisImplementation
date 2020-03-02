using Thesis.Enemy;
using System.Collections;
using UnityEngine;

public class IceZombieController : EnemyController 
{
    [SerializeField] float timeBetweenAttacksInSecs = 1f;

    private IceZombiePhysicalAttack physicalAttack;
    private bool isAttacking = false;
    private bool isReadyToAttack = true;
    private Transform Player;

    #region getters
    internal Transform GetPlayerTransform() { return player; }
    #endregion

    protected override void Start()
    {
        physicalAttack = GetComponent<IceZombiePhysicalAttack>();
        base.Start();
    }

    internal override void CheckAndUpdateEnemyState()
    {
        if (!isReadyToAttack && currentState != EnemyState.idle)
        {
            StartCoroutine(Idle());
        }
        else if (isReadyToAttack && currentState != EnemyState.attacking && playerIsInAttackDistance)
        {
            StopAllCoroutines();
            StartCoroutine(AttackPlayer());
        }
        else if (hasDetectedPlayer && currentState != EnemyState.attacking)
        {
            StartCoroutine(ChasePlayer());
        }
    }

    private IEnumerator Idle()
    {
        currentState = EnemyState.idle;
        yield return new WaitForSecondsRealtime(timeBetweenAttacksInSecs);
        isReadyToAttack = true;
    }

    private new IEnumerator AttackPlayer()
    {
        if(isAttacking)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            currentState = EnemyState.attacking;
            isAttacking = true;
            physicalAttack.PrepareToAttackPlayer();
        }
    }

    internal void EnemyStoppedAttacking()
    {
        isAttacking = false;
        isReadyToAttack = false;
        currentState = EnemyState.chasing;
    }
}
