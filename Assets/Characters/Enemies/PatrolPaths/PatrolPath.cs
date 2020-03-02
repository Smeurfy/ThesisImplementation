using UnityEngine;

[ExecuteInEditMode]
public class PatrolPath : MonoBehaviour 
{
    private int patrolPointsCount;

    private void Start()
    {
        patrolPointsCount = transform.childCount;
    }

    public Transform NextPatrolPoint(int nextPatrolPointIndex)
    {
        return transform.GetChild(nextPatrolPointIndex).transform;
    }

    public int UpdatePatrolPointIndex(int indexToUpdate)
    {
        if (indexToUpdate < patrolPointsCount - 1)
        {
            return ++indexToUpdate;
        }
        else
        {
            return 0;
        }            
    }

    private void OnDrawGizmos()
    {
        foreach(Transform patrolPoint in transform)
        {
            Gizmos.DrawWireSphere(patrolPoint.position, .1f);
        }
    }
}
