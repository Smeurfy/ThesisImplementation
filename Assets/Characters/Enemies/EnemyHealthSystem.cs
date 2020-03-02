using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Thesis.Enemy
{
    public class EnemyHealthSystem : HealthSystem
    {
        [SerializeField] private float secondsToShowDamageBar = 1f;
        [SerializeField] private GameObject HUD;
        [SerializeField] private Image remaningHealth;
        [SerializeField] private Image damageVisualization;
        [Header("OnDeathEffect")]
        [SerializeField] private GameObject bombPrefab;

        private ParticleSystem particles;
        private AudioSource audioSource;
        private AudioClip diedSoundFX;
        private bool stillAlive = true;
        private bool isBullseye = false;
        private bool hasEffectOnDeath = false;
        private bool hasBeenDamage = false;
        private EnemyController controller;

        public event Action OnEnemyDie = delegate { };
        public event Action OnEnemyTakeFirstDamage = delegate { };
        public event Action<int> OnEnemyTakeDamageFromItem = delegate { };
        public event Action<int> OnEnemyTakeDamageFromBullet = delegate { };
        public static event Action<int> OnEnemyTakeDamage = delegate { };

        private new void Start()
        {
            var enemyData = GetComponent<EnemyData>();
            audioSource = GetComponent<AudioSource>();
            diedSoundFX = enemyData.GetDiedSoundFX();
            initialHp = enemyData.GetInitialHealth();
            controller = GetComponent<EnemyController>();
            isBullseye = GetComponent<TargetController>() ? true : false;
            hasEffectOnDeath = enemyData.HasOnDeathEffect();
            InitializeHealthBar();
            base.Start();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(!particles)
            {
                particles = GetComponent<ParticleSystem>();
            }
            var incomingObject = collision.gameObject;
            if(incomingObject.GetComponent<FlyingItem>() && incomingObject.GetComponent<FlyingItem>().CanDamage())
            {
                int damageToTake = incomingObject.GetComponent<FlyingItem>().GetDamage();
                TakeDamage(damageToTake);
                OnEnemyTakeDamageFromItem(damageToTake);
            }
            else if(incomingObject.GetComponent<PlayerBullet>() && incomingObject.GetComponent<TypeOfBullet>().IsDamageImmediate())
            {
                int damageToTake = incomingObject.GetComponent<PlayerBullet>().GetDamage();
                OnEnemyTakeDamageFromBullet(damageToTake);
            }
        }

        public override void TakeDamage(int damageToTake)
        {
            if (controller)
            {
                controller.PlayerDetected();
            }
            if (!hasBeenDamage)
            {
                hasBeenDamage = true;
                OnEnemyTakeFirstDamage();
            }
            OnEnemyTakeDamage(Math.Min(damageToTake, currentHp));
            base.TakeDamage(damageToTake);
            UpdateHealthbarUI(damageToTake);
            if (particles)
            {
                particles.Play();
            }
        }

        private void UpdateHealthbarUI(int damageToTake)
        {
            if(!isBullseye)
            {
                remaningHealth.fillAmount = GetRemainingHPAsPercentage();
                StartCoroutine(UpdateDamageVisualizationBar());
            }
        }

        private IEnumerator UpdateDamageVisualizationBar()
        {
            yield return new WaitForSecondsRealtime(secondsToShowDamageBar);
            damageVisualization.fillAmount = GetRemainingHPAsPercentage();
        }

        private float GetRemainingHPAsPercentage()
        {
            return  (float)currentHp / initialHp ;
        }

        public void FellInPit()
        {
            CharacterDied();
        }

        internal override void CharacterDied()
        {
            if(stillAlive)
            {
                if(hasEffectOnDeath)
                {
                    ApplyEffectOnDeath();
                }
                if(HUD && !isBullseye)
                {
                    HUD.SetActive(false);
                }
                audioSource.PlayOneShot(diedSoundFX);
                OnEnemyDie();
                // notify that character is dead -> for roomManager 
                // notify that character is dead -> for points
                // notify that character is dead -> for performance model
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if(sr)
                {
                    sr.enabled = false;
                }
                else
                {
                    GetComponentInChildren<SpriteRenderer>().enabled = false;
                }
                controller.enabled = false;
                var bulletSpawner = GetComponentInChildren<BulletSpawner>();
                if (bulletSpawner)
                {
                    //GetComponentInChildren<EnemyShoot>().enabled = false;
                    bulletSpawner.enabled = false;
                }
                try
                {
                    GetComponent<Collider2D>().enabled = false;
                }
                catch (MissingComponentException)
                {
                    GetComponentInChildren<Collider2D>().enabled = false;
                }
                catch (NullReferenceException)
                {
                    GetComponentInChildren<Collider2D>().enabled = false;
                }
                StartCoroutine(DestroyEnemy());
                stillAlive = false;
            }
        }

        private void ApplyEffectOnDeath()
        {
            var bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            int bombDamage = 10;
            bomb.GetComponent<TypeOfBullet>().SetDamage(bombDamage);
        }

        private IEnumerator DestroyEnemy()
        {
            if(particles)
            {
                while (particles.isPlaying || audioSource.isPlaying)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            Destroy(gameObject);
        }
        
        private void InitializeHealthBar()
        {
            if(!isBullseye)
            {
                remaningHealth.fillAmount = 1;
                damageVisualization.fillAmount = 1;
            }
        }
    }
}
