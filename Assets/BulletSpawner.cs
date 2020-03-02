using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour 
{
    [SerializeField] private GameObject bulletPrefab;
    [Header("Bullet")]
    [SerializeField] private byte numberOfBullets = 25;
    [SerializeField] private byte bulletSpeed = 3;
    [Header("Wave")]
    [SerializeField] private byte numberOfWaves = 1;
    [SerializeField] private byte secondsBetweenWaves = 2;
    [Header("Shot Type")]
    [SerializeField] private bool shootAllAtTheSameTime = true;
    [SerializeField] private bool clockwise = false;
    [SerializeField] [Range(.05f, 1f)] private float secondsBetweenShots = .1f;
    [Header("Angle")]
    [SerializeField] [Range(0f, 360f)] private float angleToShootInDegrees = 360;
    [Header("Pattern")]
    [SerializeField] private bool goesBack = false;

    private float angle = 0f, angleToShootInRadius, halfAngleInRadius;
    private float angleBetweenConsecutiveBullets;
    private float sliceOfSin = 0f, angleRatio = 0f;
    private const float FULL_CIRCLE_DEGREES = 360f;
    private Vector2 direction, directionToPlayer, halfDirectionToPlayer;
    private Transform bulletParent;

    private void Start()
    {
        bulletParent = DungeonManager.instance.GetBulletHolder();
        Vector2 direction = Vector2.zero;
        Vector2 directionToPlayer = Vector2.zero;
        Vector2 intervalBetweenShots = Vector2.zero;
        angleBetweenConsecutiveBullets = (Mathf.Deg2Rad * angleToShootInDegrees) / numberOfBullets;
        sliceOfSin = (2 * Mathf.PI) / numberOfBullets;
        angleRatio = angleToShootInDegrees / FULL_CIRCLE_DEGREES;
        angleToShootInRadius = angleToShootInDegrees * Mathf.Deg2Rad;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator ShootBullets(Vector2 orientation, Vector2 playerPosition)
    {
        for(byte waveNumber = 0; waveNumber < numberOfWaves; waveNumber++)
        {
             StartCoroutine(ShootAsWave(orientation, playerPosition));
            yield return new WaitForSecondsRealtime(secondsBetweenWaves);
        }
    }

    private IEnumerator ShootAsWave(Vector2 orientation, Vector2 playerPosition)
    {
        float x, y;
        directionToPlayer = (playerPosition - (Vector2) transform.position).normalized;
        if(!goesBack)
        {
            halfAngleInRadius = angleToShootInRadius / 2;
            /*if(clockwise)
            {*/
                x = Mathf.Cos(-halfAngleInRadius) * directionToPlayer.x - Mathf.Sin(-halfAngleInRadius) * directionToPlayer.y;
                y = Mathf.Sin(-halfAngleInRadius) * directionToPlayer.x + Mathf.Cos(-halfAngleInRadius) * directionToPlayer.y;
            /*}
            else
            {
                x = Mathf.Cos(-halfAngleInRadius) * directionToPlayer.x - Mathf.Sin(-halfAngleInRadius) * directionToPlayer.y;
                y = Mathf.Sin(-halfAngleInRadius) * directionToPlayer.x + Mathf.Cos(-halfAngleInRadius) * directionToPlayer.y;
            }*/
            directionToPlayer.Set(x, y);
        }

        for (byte i = 0; i < numberOfBullets; i++)
        {
            if (!shootAllAtTheSameTime )
            {
                yield return new WaitForSecondsRealtime(secondsBetweenShots);
            }
            angle = NextAngleCalculation(i);
            x = Mathf.Cos(angle) * directionToPlayer.x - Mathf.Sin(angle) * directionToPlayer.y;
            y = Mathf.Sin(angle) * directionToPlayer.x + Mathf.Cos(angle) * directionToPlayer.y;

            direction.Set(x, y);
            direction.Normalize();

            //direction = clockwise ? CreateBullet(-direction) : CreateBullet(direction);
            direction = CreateBullet(direction);
        }
    }

    private float NextAngleCalculation(byte index)
    {
        if(goesBack)
        {
            return  Mathf.Abs( Mathf.Sin(sliceOfSin * index) )* angleRatio;
        }
        else
        { 
            return index * angleBetweenConsecutiveBullets;
        }
    }
    
    private Vector2 CreateBullet(Vector2 direction)
    {
        TypeOfBullet bullet;
        if(!(bulletPrefab.layer == (int) DefinedLayers.Bombs))
        {
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletParent).GetComponent<RegularBullet>();
            bullet.gameObject.layer = (int)DefinedLayers.EnemyBullets;
        }
        else
        {
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity, bulletParent).GetComponent<AOEBullet>();
        }
        bullet.SetSpeed(bulletSpeed);
        bullet.SetBulletVelocity(direction.normalized);
        return direction;
    }
}
