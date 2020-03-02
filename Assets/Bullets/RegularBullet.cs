using UnityEngine;

public class RegularBullet : TypeOfBullet
{
    public override void SetTarget(Vector2 target)
    {
        SetBulletDirection(target);
    }

    protected virtual void SetBulletDirection(Vector2 directionToSet) 
    {
        Vector2 movementDirection = directionToSet - (Vector2)transform.position;
        movementDirection.Normalize();
        rb.velocity = movementDirection * speed;
    }
    
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        var objectHitted = collision.gameObject.GetComponent<HealthSystem>();
        if (objectHitted)
        {
            objectHitted.TakeDamage(damage);
        }
        if(PerformanceData.instance.GetPerformanceCalculationMethod() == PerformanceCalculationMethod.basedOnHits)
        {
            if (collision.gameObject.GetComponent<PlayerHealthSystem>())
            {
                // FOR THIS TO WORK ENEMYPERFORMANCEDATA has to be added to all enemy prefabs again
                gameObject.GetComponentInParent<EnemyPerformanceData>().PlayerWasShot();
            }
        }
        Destroy(gameObject);
    }
}
