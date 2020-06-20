using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerShoot : MonoBehaviour
{
    public event Action<int> OnItemPickup = delegate { };
    public event Action OnItemThrow = delegate { };
    public static event Action<int> OnBulletFired = delegate { };
    public static event Action<int> OnBulletPickedUp = delegate { };

    [SerializeField] Weapon weaponBeingHeld;
    [SerializeField] private Transform tipOfTheGun;
    [SerializeField] private AudioClip noBulletsSound;
    [SerializeField] private Image bulletTypeImage;
    [SerializeField] private SpriteRenderer hatImage;
    [SerializeField] private GameObject[] weaponsPrefab;



    private ParticleSystem shootingPS;
    private bool isHoldingThrowable;
    private bool nextShotAvailable = true;
    private bool isGamePaused = false;
    private Animator animator;
    private Animator hatAnimator;
    private SpriteRenderer spriteRenderer;
    private BulletManager currentBulletManager;
    private AudioSource audioSource;
    private AudioClip shotSound;
    [SerializeField] private Weapon weaponBeforeChallenge;
    [SerializeField] private int bulletsBeforeChallenge;


    private const string shootButton = "Shoot Weapon";
    private const string RECOIL_TRIGGER = "recoil";
    private const string RESET_HAT_TRIGGER = "reset";

    #region getters
    public Weapon GetWeaponBeingHeld() { return weaponBeingHeld; }
    public bool IsPlayerHoldingThrowable() { return isHoldingThrowable; }
    public int GetRemainingBullets() { return currentBulletManager.AvailableBulletsCount(); }
    #endregion

    void Start()
    {
        isHoldingThrowable = weaponBeingHeld ? true : false;
        GetAttributeReferences();
        if (isHoldingThrowable)
        {
            spriteRenderer.sprite = weaponBeingHeld.GetSprite();                            // TODO change this to an observer when more weapons are implemented    
            tipOfTheGun.transform.localPosition = weaponBeingHeld.GetGunTip().transform.localPosition;
            shootingPS = Instantiate(weaponBeingHeld.GetFiringParticleSystem());
        }
        gameObject.GetComponentInParent<ThrowItem>().OnPlayerThrow += DroppedThrowable;
        PauseMenuManager.instance.OnGameIsPaused += GameIsPaused;
        SceneManager.sceneUnloaded += DereferencePause;
        AfterDeathOptions.instance.OnTryAgainNow += EnableWeapon;
        AfterDeathOptions.instance.OnSkip += EnableWeapon;
        AfterDeathOptions.instance.OnTryAgainLater += EnableWeapon;
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += BulletsBeforeChallenge;
        SceneManager.sceneLoaded += DoStuff;
    }

    private void DereferencePause(Scene sceneToUnload)
    {
        if (sceneToUnload.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            PauseMenuManager.instance.OnGameIsPaused -= GameIsPaused;
        }
    }

    public void PickedBullets(int amountOfBulletsPicked)
    {
        currentBulletManager.PickedBullets(amountOfBulletsPicked);
        OnBulletPickedUp(currentBulletManager.AvailableBulletsCount());
    }

    private void Update()
    {
        if (ShootingButtonHeldDown() && PlayerMovement.characterCanReceiveInput && weaponBeingHeld != null)
        {
            AttemptToShoot();
        }
    }

    private void AttemptToShoot()
    {
        if (nextShotAvailable && weaponBeingHeld && currentBulletManager.HasAvailableBullets() && !isGamePaused)
        {
            ShootWeapon();
        }
        else if (!currentBulletManager.HasAvailableBullets())
        {
            PlayNoBulletsSound();
        }
    }

    private void ShootWeapon()
    {
        nextShotAvailable = false;
        PlayShotSoundFX();
        TypeOfBullet bullet = Instantiate(weaponBeingHeld.GetBulletPrefab(), tipOfTheGun.position, Quaternion.identity, null);

        if (!(bullet is AOEBullet))
        {
            bullet.gameObject.layer = (int)DefinedLayers.PlayerBullets;
            var playerBullet = bullet.gameObject.AddComponent<PlayerBullet>();
            playerBullet.SetDamage(weaponBeingHeld.GetBulletDamage());
        }
        else
        {
            var typeOfBullet = bullet.GetComponent<TypeOfBullet>();
            typeOfBullet.SetDamage(weaponBeingHeld.GetBulletDamage());
            typeOfBullet.SetSpeed(20);
            typeOfBullet.SetTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        currentBulletManager.BulletFired();
        OnBulletFired(currentBulletManager.AvailableBulletsCount());
        shootingPS.Play();
        StartCoroutine(CanShootAgain());
        animator.SetTrigger(RECOIL_TRIGGER);
        //print("total shots fired: " + playerBullet.GetTotalShotsFired());
    }

    private void PlayShotSoundFX()
    {
        audioSource.pitch = UnityEngine.Random.Range(.8f, 1.1f);
        audioSource.PlayOneShot(shotSound);
    }

    private void PlayNoBulletsSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(noBulletsSound);
        }
    }

    public void ThrowablePickup(GameObject throwableToPickUp)
    {
        StopAllCoroutines();
        PickUpThrowable(throwableToPickUp);
    }

    private void DroppedThrowable()
    {
        OnItemThrow();
        weaponBeingHeld = null;
        hatImage.sprite = null;
        if (shootingPS != null)
            Destroy(shootingPS.gameObject);
        // currentBulletManager = null;
        isHoldingThrowable = false;
        nextShotAvailable = false;
        spriteRenderer.sprite = null;
    }

    private void PickUpThrowable(GameObject throwableToPickUp)
    {
        weaponBeingHeld = throwableToPickUp.GetComponent<WeaponPickup>().GetWeapon();
        weaponBeforeChallenge = weaponBeingHeld;
        shotSound = weaponBeingHeld.GetShotSound();
        bulletTypeImage.sprite = weaponBeingHeld.GetBulletTypeImage();
        hatImage.sprite = weaponBeingHeld.GetHatImage();
        hatAnimator.SetTrigger(RESET_HAT_TRIGGER);
        currentBulletManager = throwableToPickUp.GetComponent<BulletManager>();
        tipOfTheGun.transform.localPosition = weaponBeingHeld.GetGunTip().transform.localPosition;
        shootingPS = Instantiate(weaponBeingHeld.GetFiringParticleSystem(), tipOfTheGun.position, Quaternion.identity, transform);
        OnItemPickup(currentBulletManager.AvailableBulletsCount());
        isHoldingThrowable = true;
        nextShotAvailable = true;
        spriteRenderer.sprite = weaponBeingHeld.GetSprite();
    }

    private IEnumerator CanShootAgain()
    {
        yield return new WaitForSecondsRealtime(weaponBeingHeld.GetTimeToWaitToShootAgain());
        nextShotAvailable = true;
    }

    private bool ShootingButtonHeldDown()
    {
        return Input.GetButton(shootButton);
    }

    private void GetAttributeReferences()
    {
        animator = GetComponent<Animator>();                                            // TODO animator should be a property of weapon?
        hatAnimator = hatImage.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void GameIsPaused(bool isGamePaused)
    {
        this.isGamePaused = isGamePaused;
    }

    public void BulletsBeforeChallenge()
    {
        bulletsBeforeChallenge = currentBulletManager.AvailableBulletsCount();
        AfterDeathOptions.instance.UpdateBulletUI(bulletsBeforeChallenge);
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += BulletsBeforeChallenge;
    }

    private void EnableWeapon()
    {
        // Debug.Log("Enable Weapon");
        foreach (GameObject item in weaponsPrefab)
        {
            // Debug.Log("item name " + item.name);
            // Debug.Log("weapon before challenge name " + weaponBeforeChallenge.name);
            if (item.name == weaponBeforeChallenge.name && !FindObjectOfType<WeaponPickup>())
            {
                // Debug.Log("arma");
                var weapon = Instantiate(item, DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetPlayerRoomInitialPosition(), Quaternion.identity);
                weapon.GetComponent<BulletManager>().SetAvailableBullets(bulletsBeforeChallenge);
            }
        }
        //AfterDeathOptions.instance.OnTryAgain -= EnableWeapon;
    }

    private void DoStuff(Scene arg0, LoadSceneMode arg1)
    {
        isHoldingThrowable = weaponBeingHeld ? true : false;
        GetAttributeReferences();
        if (isHoldingThrowable)
        {
            spriteRenderer.sprite = weaponBeingHeld.GetSprite();                            // TODO change this to an observer when more weapons are implemented    
            tipOfTheGun.transform.localPosition = weaponBeingHeld.GetGunTip().transform.localPosition;
            shootingPS = Instantiate(weaponBeingHeld.GetFiringParticleSystem());
        }
        gameObject.GetComponentInParent<ThrowItem>().OnPlayerThrow += DroppedThrowable;
        PauseMenuManager.instance.OnGameIsPaused += GameIsPaused;
        SceneManager.sceneUnloaded += DereferencePause;
        AfterDeathOptions.instance.OnTryAgainNow += EnableWeapon;
        AfterDeathOptions.instance.OnSkip += EnableWeapon;
        DungeonManager.instance.GetRoomManagerByRoomID(DungeonManager.instance.playersRoom).GetDoorHolder().GetComponentInChildren<DoorManager>().OnPlayerEnteredRoom += BulletsBeforeChallenge;
    }
}
