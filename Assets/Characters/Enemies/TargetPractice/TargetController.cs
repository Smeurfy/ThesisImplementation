using UnityEngine;
using System.Collections;

namespace Thesis.Enemy
{
    public class TargetController : EnemyController
    {
        EnemyMovement movement;

        private new void Start()
        {
            movement = GetComponent<EnemyMovement>();
            movement.Patrol();
        }

        private void Update()
        {
            StartCoroutine(Patrol());
        }

        private IEnumerator Patrol()
        {
            movement.Patrol();
            yield return new WaitForEndOfFrame();
        }
    }
}
