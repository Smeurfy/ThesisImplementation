using System.Collections;
using UnityEngine;

namespace Thesis.Enemy
{
    public class EnemyShootTest : MonoBehaviour
    {
        [SerializeField] Transform gunTip;

        public float timeToWaitBeforeShootingAgain;
        private bool canShoot;
        private bool isLookingToTheRight = true;
        private AudioSource audioSource;
        private AudioClip enemyShot;
        private EnemyControllerTest controller;
        private BulletSpawner shooter;

        void Awake(){
            var enemyData = GetComponentInParent<EnemyData>();
            audioSource = GetComponent<AudioSource>();
            controller = GetComponentInParent<EnemyControllerTest>();
            shooter = GetComponent<BulletSpawner>();
            enemyShot = enemyData.GetShotSoundFX();
            canShoot = Random.Range(0, 3) == 0 ? false : true;
            if(!canShoot)
            {
                timeToWaitBeforeShootingAgain = Random.Range(1f, 1.5f);
                StartCoroutine(CanShootAgain());
            }
            timeToWaitBeforeShootingAgain = enemyData.GetSecondsBetweenShots();
        }

        void Start()
        {
            
        }
        
        internal void AttemptToShoot()
        {
            if (canShoot)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            CheckOrientation();
            audioSource.pitch = Random.Range(.9f, 1.1f);
            audioSource.PlayOneShot(enemyShot);
            Shoot(gunTip);
            canShoot = false;
            StartCoroutine(CanShootAgain());
        }

        public void Shoot(Transform gunTip)
        {
            if(isLookingToTheRight)
            {
                StartCoroutine(shooter.ShootBullets(Vector2.right, controller.GetPlayerHealthSystem().transform.position));
            }
            else
            {
                StartCoroutine(shooter.ShootBullets(Vector2.left, controller.GetPlayerHealthSystem().transform.position));
            }
            /*
            var bullet = Instantiate(weaponBeingHeld.GetBulletPrefab(), gunTip.position, Quaternion.identity, gameObject.transform);
            if (!(bullet is AOEBullet))
            { 
                bullet.gameObject.layer = (int)DefinedLayers.EnemyBullets;
                bullet.gameObject.AddComponent<EnemyBullet>();
            }
            else
            {
                var bulletType = bullet.GetComponent<TypeOfBullet>();
                bulletType.SetDamage(weaponBeingHeld.GetBulletDamage());
                bulletType.SetSpeed(5);
                
            }
            bullet.SetTarget(GameManager.instance.GetPlayerReference().transform.position);
            */
        }
        
        private void CheckOrientation()
        {
            float orientation = gunTip.transform.position.x - controller.GetPlayerHealthSystem().transform.position.x;
            bool currentOrientation = (orientation < 0) ? true : false;
            if (currentOrientation != isLookingToTheRight)
            {
                Vector3 currentGunPosition = gunTip.transform.localPosition;
                gunTip.transform.localPosition = currentGunPosition * (-1f);
                isLookingToTheRight = !isLookingToTheRight;
            }
        }

        private IEnumerator CanShootAgain()
        {
            yield return new WaitForSecondsRealtime(timeToWaitBeforeShootingAgain);
            canShoot = true;
        }
    }
}
