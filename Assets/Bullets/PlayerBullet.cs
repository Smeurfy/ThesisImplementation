using UnityEngine;

public class PlayerBullet : MonoBehaviour 
{
    [SerializeField] private float speed = 20;

    private static int totalNumberOfShotsFired = 0;
    private int damageToDealtToEnemy;
    private TypeOfBullet typeOfBullet;

    #region getters
    public int GetTotalShotsFired() { return totalNumberOfShotsFired; }
    #endregion

    void Start()
    {
        totalNumberOfShotsFired++;
        typeOfBullet = GetComponent<TypeOfBullet>();
        typeOfBullet.SetSpeed(speed);
        typeOfBullet.SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void SetDamage(int bulletDamage)
    {
        damageToDealtToEnemy = bulletDamage;
        if(typeOfBullet == null)
        {
            typeOfBullet = GetComponent<TypeOfBullet>();
        }
        typeOfBullet.SetDamage(damageToDealtToEnemy);
    }

    public int GetDamage()
    {
        return damageToDealtToEnemy;
    }
}
