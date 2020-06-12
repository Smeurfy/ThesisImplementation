using UnityEngine;

public class RegularBullet : TypeOfBullet
{
    private void Start(){
        AfterDeathOptions.instance.OnTryAgainNow += DestroyBullet;
        AfterDeathOptions.instance.OnSkip += DestroyBullet;
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
        Destroy(gameObject);
    }

    private void DestroyBullet(){
        if(this){
            Destroy(gameObject);
        }
    }
}
