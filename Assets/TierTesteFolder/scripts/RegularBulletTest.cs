using UnityEngine;

public class RegularBulletTest : TypeOfBullet
{
    private void Start(){
        // AfterDeathOptions.instance.OnTryAgain += DestroyBullet;
        // AfterDeathOptions.instance.OnSkip += DestroyBullet;
    }

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
        //if(performancedata.instance.getperformancecalculationmethod() == performancecalculationmethod.basedonhits)
        //{
        //    if (collision.gameobject.getcomponent<playerhealthsystem>())
        //    {
        //        // for this to work enemyperformancedata has to be added to all enemy prefabs again
        //        gameobject.getcomponentinparent<enemyperformancedata>().playerwasshot();
        //    }
        //}
        Destroy(gameObject);
    }

    private void DestroyBullet(){
        if(this){
            Destroy(gameObject);
        }
    }
}
