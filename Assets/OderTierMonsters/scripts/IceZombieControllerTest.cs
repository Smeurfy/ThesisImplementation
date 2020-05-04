using Thesis.Enemy;
using System.Collections;
using UnityEngine;

public class IceZombieControllerTest : EnemyControllerTest 
{
    [SerializeField]
    public float timeBetweenAttacksInSecs = 1f;

    private IceZombiePhysicalAttack physicalAttack;
    private bool isAttacking = false;
    private bool isReadyToAttack = true;

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
        if (!isReadyToAttack && currentState != EnemyStateTest.idle)
        {
            StartCoroutine(Idle());
        }
        else if (isReadyToAttack && currentState != EnemyStateTest.attacking && playerIsInAttackDistance)
        {
            StopAllCoroutines();
            StartCoroutine(AttackPlayer());
        }
        else if (hasDetectedPlayer && currentState != EnemyStateTest.attacking)
        {
            StartCoroutine(ChasePlayer());
        }
    }

    private IEnumerator Idle()
    {
        currentState = EnemyStateTest.idle;
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
            currentState = EnemyStateTest.attacking;
            isAttacking = true;
            physicalAttack.PrepareToAttackPlayer();
        }
    }

    internal void EnemyStoppedAttacking()
    {
        isAttacking = false;
        isReadyToAttack = false;
        currentState = EnemyStateTest.chasing;
    }
}
