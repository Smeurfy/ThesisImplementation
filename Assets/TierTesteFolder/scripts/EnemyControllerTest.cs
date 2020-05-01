using System.Collections;
using UnityEngine;

public enum EnemyStateTest { idle, attacking, patrolling, chasing, resting, beingPushed}

namespace Thesis.Enemy
{
    public class EnemyControllerTest : MonoBehaviour
    {
        [SerializeField] protected Transform player;
        [SerializeField] public float attackDistance;
        [SerializeField] protected float durationOfPush;
        [SerializeField] protected bool isStationaryEnemy = false;
        
        protected EnemyMovementTest enemyMovement;
        protected EnemyShootTest enemyShoot;
        protected bool hasDetectedPlayer = false;
        protected bool playerIsInStoppingDistance = false;
        protected bool playerIsInAttackDistance = false;
        protected EnemyStateTest currentState = EnemyStateTest.idle;

        #region getters
        internal PlayerHealthSystemTest GetPlayerHealthSystem() { return player.GetComponent<PlayerHealthSystemTest>(); }
        #endregion

        protected virtual void Start()
        {
            enemyMovement = GetComponent<EnemyMovementTest>();
            enemyShoot = GetComponentInChildren<EnemyShootTest>();
            player = FindObjectOfType<PlayerHealthSystemTest>().transform;
        }

        void Update()
        {
            if(!isStationaryEnemy)
            {
                if (currentState != EnemyStateTest.beingPushed)
                {
                    playerIsInStoppingDistance = Vector2.Distance(player.position, transform.position) < enemyMovement.GetStoppingDistance();
                    playerIsInAttackDistance = Vector2.Distance(player.position, transform.position) < attackDistance;
                    CheckAndUpdateEnemyState();
                }
            }
            else
            {
                if(currentState == EnemyStateTest.patrolling)
                {
                    enemyMovement.StopMoving(player);
                }
            }
        }

        internal virtual void CheckAndUpdateEnemyState()
        {
            if (!hasDetectedPlayer && currentState != EnemyStateTest.patrolling)
            {
                StartCoroutine(Patrolling());
            }
            if (playerIsInAttackDistance && currentState != EnemyStateTest.attacking)
            {
                StopAllCoroutines();
                currentState = EnemyStateTest.attacking;
                AttackPlayer();
            }
            if( hasDetectedPlayer )
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
        }
        
        public void Pushed()
        {
            currentState = EnemyStateTest.beingPushed;
            StopAllCoroutines();
            StartCoroutine(ResetBeingPushed());
        }

        private IEnumerator ResetBeingPushed()
        {
            yield return new WaitForSecondsRealtime(durationOfPush);
            currentState = EnemyStateTest.patrolling;
        }

        protected virtual void AttackPlayer()
        {
            hasDetectedPlayer = true;
            if (!playerIsInAttackDistance)
            {
                currentState = EnemyStateTest.chasing;
            }
            else
            {
                enemyShoot.AttemptToShoot();
            }
        }

        protected IEnumerator Patrolling()
        {
            currentState = EnemyStateTest.patrolling;
            while(true)
            {
                enemyMovement.Patrol();
                yield return new WaitForEndOfFrame();
            }
        }

        protected IEnumerator ChasePlayer()
        {
            currentState = EnemyStateTest.chasing;
            hasDetectedPlayer = true;
            if (!playerIsInStoppingDistance)
            {
                enemyMovement.MoveTowards(player);
            }
            else
            {
                enemyMovement.StopMoving(player);
            }
            yield return new WaitForEndOfFrame();
        }

        internal void PlayerDetected()
        {
            hasDetectedPlayer = true;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }
}