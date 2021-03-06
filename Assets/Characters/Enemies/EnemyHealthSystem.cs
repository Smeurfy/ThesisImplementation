﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

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
        private EnemyController controller;
        private bool cdDamage = false;

        public EnemyData enemyData;

        public event Action OnEnemyDie = delegate { };
        public static event Action<float> OnEnemyTakeDamage = delegate { };

        private new void Start()
        {
            enemyData = GetComponent<EnemyData>();
            audioSource = GetComponent<AudioSource>();
            diedSoundFX = enemyData.GetDiedSoundFX();
            initialHp = enemyData.GetInitialHealth();
            controller = GetComponent<EnemyController>();
            isBullseye = GetComponent<TargetController>() ? true : false;
            hasEffectOnDeath = enemyData.HasOnDeathEffect();
            InitializeHealthBar();
            base.Start();
            AfterDeathOptions.instance.OnTryAgainNow += EnableVariables;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!particles)
            {
                particles = GetComponent<ParticleSystem>();
            }
            var incomingObject = collision.gameObject;
            if (!cdDamage && incomingObject.GetComponent<FlyingItem>() && incomingObject.GetComponent<FlyingItem>().CanDamage())
            {
                cdDamage = true;
                int damageToTake = incomingObject.GetComponent<FlyingItem>().GetDamage();
                TakeDamage(damageToTake);
                StartCoroutine(WaitToTakeDmg());
            }
            else if (incomingObject.GetComponent<PlayerBullet>() && incomingObject.GetComponent<TypeOfBullet>().IsDamageImmediate())
            {
                int damageToTake = incomingObject.GetComponent<PlayerBullet>().GetDamage();
            }
        }

        private IEnumerator WaitToTakeDmg()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            cdDamage = false;
        }

        public override void TakeDamage(int damageToTake)
        {
            if (controller)
            {
                controller.PlayerDetected();
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
            if (!isBullseye)
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
            return (float)currentHp / initialHp;
        }

        internal override void CharacterDied()
        {
            if (stillAlive)
            {
                if (GetComponentsInChildren<ParticleSystem>().Length > 1 && GetComponentsInChildren<ParticleSystem>()[1] != null)
                {
                    Destroy(GetComponentsInChildren<ParticleSystem>()[1]);
                }
                if (gameObject.name == "iceZombieTest")
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                if (HUD && !isBullseye)
                {
                    HUD.SetActive(false);
                }
                audioSource.PlayOneShot(diedSoundFX);
                OnEnemyDie();
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr)
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
            EnablePopUpPoints();
            if (particles)
            {
                while (particles.isPlaying || audioSource.isPlaying)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            Destroy(gameObject);

        }

        private void EnablePopUpPoints()
        {
            var enemiesTier = DungeonManager.instance.tierOfEnemies;
            try
            {
                foreach (TypeOfEnemy enemy in DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).challengeOfThisRoom.GetTypeOfEnemies())
                {

                    if ((enemy.name + "Test") == gameObject.name)
                    {
                        if (enemiesTier[enemy] == 0 || enemiesTier[enemy] == 1)
                        {
                            Instantiate(DungeonManager.instance.pointsPopup[0], transform.position, Quaternion.identity);
                            HighScore.instance.UpdateScore(10);
                        }
                        if (enemiesTier[enemy] == 2 || enemiesTier[enemy] == 3)
                        {
                            Instantiate(DungeonManager.instance.pointsPopup[1], transform.position, Quaternion.identity);
                            HighScore.instance.UpdateScore(20);
                        }
                        if (enemiesTier[enemy] == 4)
                        {
                            Instantiate(DungeonManager.instance.pointsPopup[2], transform.position, Quaternion.identity);
                            HighScore.instance.UpdateScore(30);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                //bah
            }
        }

        private void InitializeHealthBar()
        {
            if (!isBullseye)
            {
                remaningHealth.fillAmount = 1;
                damageVisualization.fillAmount = 1;
            }
        }

        private void EnableVariables()
        {
            stillAlive = true;
        }
    }
}
