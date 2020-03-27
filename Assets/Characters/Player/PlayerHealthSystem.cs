using System;
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
        SceneManager.sceneLoaded += GetFlashingDamageIndicatorReference;
        MakeThisObjectSingleton();
        DontDestroyOnLoad(gameObject);
    }

    new private void Start()
    {
        AfterDeathOptions.instance.OnTryAgain += EnablePlayer;
        GetReferencesToAttributes();
        initialHp = startingHp;
        base.Start();
        OnPlayerHealthUpdate(currentHp);
    }
    
    internal override void CharacterDied()
    {
        AfterDeathOptions.instance.afterDeathMenu.SetActive(true);
        audioSource.PlayOneShot(soundFXHolder.GetDiedSound(), 1f);
        if(!alreadySignaledPlayerDeath)
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
        gameObject.SetActive(false);
    }

    public void EnablePlayer()
    {
        alreadySignaledPlayerDeath = false;
        GetComponent<PlayerMovement>().enabled = true;
        GetComponentInChildren<PlayerShoot>().enabled = true;
        GetComponent<ThrowItem>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        currentHp = hpBeforeChallenge;
        OnPlayerHealthUpdate(currentHp);
        transform.position = DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetPlayerRoomInitialPosition();
        canTakeDamage = true;
        gameObject.SetActive(true);
    }

    public void EnablePlayerControls()
    {
        PlayerCanControlCharacter(true);
    }

    private void PlayerCanControlCharacter(bool state)
    {
        GetComponent<PlayerMovement>().enabled = state;
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
        if(!shieldIsBeingUsed)
        { 
            if(canTakeDamage)
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
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        soundFXHolder = GetComponent<AudioClipHolder>();
        flashingScreenOnDamage.enabled = false;
    }

    private void GetFlashingDamageIndicatorReference(Scene arg0, LoadSceneMode arg1)
    {
        currentHp = startingHp;
        flashingScreenOnDamage = Camera.main.GetComponentInChildren<Image>();
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
