using UnityEngine;

public class EnemyBullet : MonoBehaviour 
{
    [SerializeField] private float speed = 4;

    private int damageToDealToThePlayer = 1;
    private TypeOfBullet bulletType;

    void Awake()
    {
        bulletType = GetComponent<TypeOfBullet>();
        bulletType.SetSpeed(speed);
    }
}
