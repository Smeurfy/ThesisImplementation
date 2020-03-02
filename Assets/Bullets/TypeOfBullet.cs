using UnityEngine;

public abstract class TypeOfBullet : MonoBehaviour 
{
    protected float speed;
    protected Rigidbody2D rb;
    protected bool isDamageImmediate = true;
    protected int damage = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDamage(int damage) { this.damage = damage; }
    public abstract void SetTarget(Vector2 target);
    protected abstract void OnCollisionEnter2D(Collision2D collision);

    public void SetSpeed(float newSpeed) { speed = newSpeed; }
    public bool IsDamageImmediate() { return isDamageImmediate; }

    public void SetBulletVelocity(Vector2 velocity)
    {
        rb.velocity = velocity * speed;
    }
}
