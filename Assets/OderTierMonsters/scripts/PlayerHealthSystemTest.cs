using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Thesis.Enemy;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthSystemTest : HealthSystem
{
    public static PlayerHealthSystemTest instance;

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
        SceneManager.sceneLoaded += GetFlashingDamageIndicatorReference;
        MakeThisObjectSingleton();
        DontDestroyOnLoad(gameObject);
    }

    new private void Start()
    {
        GetReferencesToAttributes();
        initialHp = startingHp;
        base.Start();
        OnPlayerHealthUpdate(currentHp);
        OnPlayerDied += DestroyWeapons;
    }

    internal override void CharacterDied()
    {
        audioSource.PlayOneShot(soundFXHolder.GetDiedSound(), 1f);
        if (!alreadySignaledPlayerDeath)
        {
            alreadySignaledPlayerDeath = true;
            OnPlayerDied();
            StartCoroutine(DisablePlayer());
        }
    }

    private IEnumerator DisablePlayer()
    {
        //Camera.main.GetComponentInChildren<Animator>().enabled = true;
        PlayerCanControlCharacter(false);
        canTakeDamage = false;
        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        GetComponentInChildren<PlayerShootTest>().DroppedThrowable();
        gameObject.SetActive(false);
    }

    public void EnablePlayer()
    {
        if (this != null)
        {
            PlayerMovement.characterCanReceiveInput = true;
            alreadySignaledPlayerDeath = false;
            GetComponent<PlayerMovement>().enabled = true;
            GetComponentInChildren<PlayerShootTest>().enabled = true;
            GetComponent<ThrowItemTest>().enabled = true;
            GetComponent<Collider2D>().enabled = true;
            currentHp = hpBeforeChallenge;
            OnPlayerHealthUpdate(currentHp);
            transform.position = GameObject.Find("room specific").GetComponent<RoomManager>().GetPlayerRoomInitialPosition();
            canTakeDamage = true;
            GetComponentInChildren<PlayerShootTest>().weaponBeingHeld = null;
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
        GetComponentInChildren<PlayerShootTest>().enabled = state;
        GetComponent<ThrowItemTest>().enabled = state;
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
        enemy.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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

    private void GetFlashingDamageIndicatorReference(Scene arg0, LoadSceneMode arg1)
    {
        flashingScreenOnDamage = Camera.main.GetComponentInChildren<Image>();
        GetReferencesToAttributes();
        initialHp = startingHp;
        base.Start();
        OnPlayerHealthUpdate(currentHp);
        OnPlayerDied += DestroyWeapons;

        currentHp = startingHp;

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
