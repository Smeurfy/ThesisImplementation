using UnityEngine;
using Thesis.Enemy;

public class SpikeBehaviour : MonoBehaviour 
{
    [SerializeField] private int damageToDeal = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var objectCollided = collision.gameObject;
        if (objectCollided.GetComponent<PlayerMovement>())
        {
            print("player fell in the pit");    // TODO damage player
        }
        else if (objectCollided.GetComponent<EnemyController>())
        {
            objectCollided.GetComponent<EnemyHealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
