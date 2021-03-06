﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Thesis.Enemy;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthSystem : HealthSystem
{
    public static PlayerHealthSystem instance;

    [SerializeField] private int startingHp;
    [SerializeField] private Image flashingScreenOnDamage;

    public event Action OnPlayerDied = delegate { };
    public event Action<int> OnPlayerHealthUpdate = delegate { };

    private Animator animator;
    private AudioSource audioSource;
    private AudioClipHolder soundFXHolder;
    private ShieldManager shieldManager;
    private bool canTakeDamage = true;
    private bool shieldIsBeingUsed = false;
    private bool alreadySignaledPlayerDeath = false;
    private float secondsOfInvincibility = 1f;

    private const string animatorGotHit = "gotHit";

    private void Awake()
    {
        shieldManager = GetComponent<ShieldManager>();
        shieldManager.OnShieldActivation += UpdateShieldState;
        shieldIsBeingUsed = false;
        MakeThisObjectSingleton();
        DontDestroyOnLoad(gameObject);
    }

    new private void Start()
    {
        SceneManager.sceneLoaded += GetFlashingDamageIndicatorReference;
        AfterDeathOptions.instance.OnTryAgainNow += EnablePlayer;
        AfterDeathOptions.instance.OnSkip += EnablePlayer;
        AfterDeathOptions.instance.OnTryAgainLater += EnablePlayer;
        GetReferencesToAttributes();
        initialHp = startingHp;
        base.Start();
        OnPlayerHealthUpdate(currentHp);
        FindObjectOfType<UIHealthManager>().UpdateHealthText(currentHp);
        OnPlayerDied += DestroyWeapons;
        OnPlayerDied += GameManager.instance.GetComponentInChildren<ScoreManager>().UndoScore;
        HealthBonus.instance.OnHeartFull += GainOneHeart;
    }

    private void OnEnable()
    {
        // StartCoroutine(TryAssignReference());
    }

    private IEnumerator TryAssignReference()
    {
        yield return new WaitForSeconds(0.2f);
        try
        {
            HighScore.instance._player = gameObject;
        }
        catch (NullReferenceException)
        {
            TryAssignReference();
        }
    }

    private void GainOneHeart()
    {
        currentHp++;
        OnPlayerHealthUpdate(currentHp);
        FindObjectOfType<UIHealthManager>().UpdateHealthText(currentHp);
    }

    internal override void CharacterDied()
    {
        audioSource.PlayOneShot(soundFXHolder.GetDiedSound(), 1f);
        if (!alreadySignaledPlayerDeath)
        {
            alreadySignaledPlayerDeath = true;
            OnPlayerDied();
            StartCoroutine(DisablePlayer());
            JsonWriter.instance._roomsOfDeath.Add(DungeonManager.instance.indexChallenge);
            JsonWriter.instance._health.Add(hpBeforeChallenge);
        }
    }

    private IEnumerator DisablePlayer()
    {
        PlayerCanControlCharacter(false);
        canTakeDamage = false;
        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        AfterDeathOptions.instance.UpdateHealthUI(hpBeforeChallenge);
        AfterDeathOptions.instance.afterDeathMenu.SetActive(true);
    }

    public void EnablePlayer()
    {
        if (this != null)
        {
            alreadySignaledPlayerDeath = false;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponentInChildren<PlayerShoot>().enabled = true;
            GetComponent<ThrowItem>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            currentHp = hpBeforeChallenge;
            OnPlayerHealthUpdate(currentHp);
            FindObjectOfType<UIHealthManager>().UpdateHealthText(currentHp);
            transform.position = DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetPlayerRoomInitialPosition();
            canTakeDamage = true;
            gameObject.SetActive(true);
        }
    }

    public void EnablePlayerControls()
    {
        PlayerCanControlCharacter(true);
    }

    private void PlayerCanControlCharacter(bool state)
    {
        //GetComponent<PlayerMovement>().enabled = state;
        PlayerMovement.characterCanReceiveInput = state;
        GetComponentInChildren<PlayerShoot>().enabled = state;
        GetComponent<ThrowItem>().enabled = state;
        GetComponent<Collider2D>().enabled = state;
        if (state)
        {
            GetComponent<Collider2D>().enabled = true;
            transform.position = GameManager.instance.GetRespawnPosition();
            gameObject.SetActive(true);
            StartCoroutine(ResetSignalPlayerDead());
            canTakeDamage = true;
        }
    }

    private IEnumerator ResetSignalPlayerDead()
    {
        yield return new WaitForSecondsRealtime(5f);
        alreadySignaledPlayerDeath = false;
    }

    public override void TakeDamage(int damageToTake)   // player ALWAYS takes 1 point of damage
    {
        if (!shieldIsBeingUsed)
        {
            if (canTakeDamage)
            {
                canTakeDamage = false;
                base.TakeDamage(1);
                flashingScreenOnDamage.enabled = true;
                StartCoroutine(TurnOffFlashingScreenOnDamage());
                StartCoroutine(ResetInvincibility());
                animator.SetTrigger(animatorGotHit);
                audioSource.PlayOneShot(soundFXHolder.GetGotDamagedSound());
                OnPlayerHealthUpdate(currentHp);
                FindObjectOfType<UIHealthManager>().UpdateHealthText(currentHp);
            }
        }
    }

    private IEnumerator ResetInvincibility()
    {
        yield return new WaitForSecondsRealtime(secondsOfInvincibility);
        canTakeDamage = true;
    }

    private IEnumerator TurnOffFlashingScreenOnDamage()
    {
        yield return new WaitForSecondsRealtime(.1f);
        flashingScreenOnDamage.enabled = false;
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collidedObject = collision.gameObject;
        if (collidedObject.GetComponent<EnemyController>())
        {
            TakeCollisionDamageFromEnemy(collidedObject);
        }
    }

    private void TakeCollisionDamageFromEnemy(GameObject enemy)
    {
        TakeDamage(1);
        // enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private void UpdateShieldState(bool shieldState)
    {
        shieldIsBeingUsed = shieldState;
    }

    private void GetReferencesToAttributes()
    {
        if (this != null)
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            soundFXHolder = GetComponent<AudioClipHolder>();
            flashingScreenOnDamage.enabled = false;
        }
    }

    private void GetFlashingDamageIndicatorReference(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            EnablePlayerControls();
            AfterDeathOptions.instance.OnTryAgainNow += EnablePlayer;
            AfterDeathOptions.instance.OnSkip += EnablePlayer;
            AfterDeathOptions.instance.OnTryAgainLater += EnablePlayer;
            flashingScreenOnDamage = Camera.main.GetComponentInChildren<Image>();
            GetReferencesToAttributes();
            initialHp = startingHp;
            base.Start();
            OnPlayerHealthUpdate(currentHp);
            FindObjectOfType<UIHealthManager>().UpdateHealthText(currentHp);
            OnPlayerDied += DestroyWeapons;
            HealthBonus.instance.OnHeartFull += GainOneHeart;

            currentHp = startingHp;
        }
    }

    private void DestroyWeapons()
    {
        var objs = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (var item in objs)
        {
            Destroy(item);
        }
    }

    private void MakeThisObjectSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
