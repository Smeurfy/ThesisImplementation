using System.Collections;
using UnityEngine;

public enum EnemyState { idle, attacking, patrolling, chasing, resting, beingPushed}

namespace Thesis.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] protected Transform player;
        [SerializeField] protected float attackDistance;
        [SerializeField] protected float durationOfPush;
        [SerializeField] protected bool isStationaryEnemy = false;
        
        protected EnemyMovement enemyMovement;
        protected EnemyShoot enemyShoot;
        protected bool hasDetectedPlayer = false;
        protected bool playerIsInStoppingDistance = false;
        protected bool playerIsInAttackDistance = false;
        protected EnemyState currentState = EnemyState.idle;

        #region getters
        internal PlayerHealthSystem GetPlayerHealthSystem() { return player.GetComponent<PlayerHealthSystem>(); }
        #endregion

        protected virtual void Start()
        {
            enemyMovement = GetComponent<EnemyMovement>();
            enemyShoot = GetComponentInChildren<EnemyShoot>();
            player = GameManager.instance.GetPlayerReference();
        }

        void Update()
        {
            if(!isStationaryEnemy)
            {
                if (currentState != EnemyState.beingPushed)
                {
                    playerIsInStoppingDistance = Vector2.Distance(player.position, transform.position) < enemyMovement.GetStoppingDistance();
                    playerIsInAttackDistance = Vector2.Distance(player.position, transform.position) < attackDistance;
                    CheckAndUpdateEnemyState();
                }
            }
            else
            {
                if(currentState == EnemyState.patrolling)
                {
                    enemyMovement.StopMoving(player);
                }
            }
        }

        internal virtual void CheckAndUpdateEnemyState()
        {
            if (!hasDetectedPlayer && currentState != EnemyState.patrolling)
            {
                StartCoroutine(Patrolling());
            }
            if (playerIsInAttackDistance && currentState != EnemyState.attacking)
            {
                StopAllCoroutines();
                currentState = EnemyState.attacking;
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
            currentState = EnemyState.beingPushed;
            StopAllCoroutines();
            StartCoroutine(ResetBeingPushed());
        }

        private IEnumerator ResetBeingPushed()
        {
            yield return new WaitForSecondsRealtime(durationOfPush);
            currentState = EnemyState.patrolling;
        }

        protected virtual void AttackPlayer()
        {
            hasDetectedPlayer = true;
            if (!playerIsInAttackDistance)
            {
                currentState = EnemyState.chasing;
            }
            else
            {
                enemyShoot.AttemptToShoot();
            }
        }

        protected IEnumerator Patrolling()
        {
            currentState = EnemyState.patrolling;
            while(true)
            {
                enemyMovement.Patrol();
                yield return new WaitForEndOfFrame();
            }
        }

        protected IEnumerator ChasePlayer()
        {
            currentState = EnemyState.chasing;
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