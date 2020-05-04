using System.Collections;
using UnityEngine;
using Thesis.Enemy;

public class IceZombiePhysicalAttack : EnemyPhysicalAttack 
{
    [SerializeField]public float DurationOfAttackInSecs = 2f;
    [SerializeField] public float attackFlightSpeed = 5f;
    [SerializeField] private Animator animator;
    
    private IceZombieControllerTest iceZombieController;
    private EnemyMovementTest movement;
    private bool preparingToAttackNotSet = true;

    private const string animatorPreparingAttack = "preparingAttack";
    private const string animatorIsAttacking = "isAttacking";

    private void Start()
    {
        iceZombieController = GetComponent<IceZombieControllerTest>();
        movement = GetComponent<EnemyMovementTest>();
    }

    internal void PrepareToAttackPlayer()
    {
        if(preparingToAttackNotSet)
        {
            animator.SetTrigger(animatorPreparingAttack);
            preparingToAttackNotSet = false;
        }
        StartCoroutine(Attack());        
    }

    private IEnumerator Attack()
    {
        LaunchEnemyTowardsThePlayer();
        yield return new WaitForSecondsRealtime(DurationOfAttackInSecs);
        StopAttack();
    }

    private void LaunchEnemyTowardsThePlayer()
    {
        animator.SetBool(animatorIsAttacking, true);
        movement.MoveTowards(iceZombieController.GetPlayerTransform(), attackFlightSpeed);
    }

    private void StopAttack()
    {
        movement.StopMoving(iceZombieController.GetPlayerTransform());
        animator.SetBool(animatorIsAttacking, false);
        iceZombieController.EnemyStoppedAttacking();
        preparingToAttackNotSet = true;
    }
}
