using UnityEngine;

namespace Thesis.Enemy
{
    public class EnemyMovementTest : MonoBehaviour
    {
        [SerializeField] private float stoppingDistanceToPlayer;
        [SerializeField] public float movementSpeed;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float stoppingDistanceToPatrolPoint;

        private Rigidbody2D rb;
        private Animator animator;
        private Transform patrolPointToGoTo;
        private int patrolPointIndex;
        
        private const string animatorOrientation = "orientation";
        private const string animatorSpeed = "movementSpeed";

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            if(patrolPath)
            {
                PatrolPathSetup();
            }
        }

        private void Update()
        {
            UpdateEnemyOrientation(FindObjectOfType<PlayerHealthSystem>().transform.position - transform.position);
        }

        private void PatrolPathSetup()
        {
            patrolPointIndex = 0;
            patrolPointToGoTo = patrolPath.NextPatrolPoint(patrolPointIndex);
        }

        #region getters
        public float GetStoppingDistance() { return stoppingDistanceToPlayer; }
        #endregion

        internal void MoveTowards(Transform target)
        {
            Vector3 newVelocity = (target.position - transform.position).normalized;
            rb.velocity = newVelocity * movementSpeed;
            UpdateMovementSpeedInAnimator();
        }

        internal void MoveTowards(Transform target, float speed)
        {
            Vector3 newVelocity = (target.position - transform.position).normalized;
            rb.velocity = newVelocity * speed;
            UpdateMovementSpeedInAnimator();
        }

        internal void StopMoving(Transform player)
        {
            rb.velocity = Vector2.zero;
            UpdateMovementSpeedInAnimator();
        }

        internal void Patrol()
        {
            if(patrolPath)
            {
                bool isCloseEnoughToPatrolPoint = stoppingDistanceToPatrolPoint > Vector2.Distance(transform.position, patrolPointToGoTo.position);
                if (isCloseEnoughToPatrolPoint)
                {
                    UpdatePatrolPoint();
                }
                else
                {
                    MoveTowards(patrolPointToGoTo);
                }
            }
        }

        private void UpdatePatrolPoint()
        {
            patrolPointIndex = patrolPath.UpdatePatrolPointIndex(patrolPointIndex);
            patrolPointToGoTo = patrolPath.NextPatrolPoint(patrolPointIndex);
            UpdateEnemyOrientation(patrolPointToGoTo.position);
        }

        public void UpdateEnemyOrientation(Vector3 positionToFace)
        {
            float orientation = positionToFace.x > 0 ? 1 : 0;

            if(animator)
            {
                animator.SetFloat(animatorOrientation, orientation);
            }
        }

        private void UpdateMovementSpeedInAnimator()
        {
            if(animator)
            {
                animator.SetFloat(animatorSpeed, rb.velocity.magnitude);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, stoppingDistanceToPlayer);
        }
    }
}
