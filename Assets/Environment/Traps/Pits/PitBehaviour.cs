using UnityEngine;
using Thesis.Enemy;

public class PitBehaviour : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var objectCollided = collision.gameObject;
        if (objectCollided.GetComponent<PlayerMovement>())
        {
            print("player fell in the pit");    // TODO damage player
        }
        else if (objectCollided.GetComponent<EnemyController>())
        {
            objectCollided.GetComponent<EnemyHealthSystem>().FellInPit();
        }
    }
}
