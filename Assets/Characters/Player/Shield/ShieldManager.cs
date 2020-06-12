using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShieldManager : MonoBehaviour
{
    public event Action<bool> OnShieldActivation = delegate { };

    [SerializeField] private float timeToDepleteShield = 2f;
    [SerializeField] private GameObject shield;
    [SerializeField] private AudioClip shieldEnabled, shieldDisabled, shieldUnlocked;
    [SerializeField] private BulletDrop bulletDropPrefab;

    private ShieldUIManager shieldUI;
    private ShieldController shieldController;
    private AudioSource audioPlayer;
    private const string activateShieldButton = "Use Shield";
    public static bool isShieldUnlocked = false;
    private bool shieldIsCharged = false;

    private void Start()
    {
        DontDestroyOnLoad(this);
        audioPlayer = GetComponent<AudioSource>();
        shieldController = GetComponentInChildren<ShieldController>();
        shieldController.gameObject.SetActive(false);
        if (shieldUI == null)
        {
            shieldUI = FindObjectOfType<ShieldUIManager>();
        }
        shieldUI.gameObject.SetActive(false);
        ScoreManager.OnReachedShieldUnlockRoom += UnlockShield;
        AfterDeathOptions.instance.OnTryAgainNow += ResetShield;
        AfterDeathOptions.instance.OnSkip += ResetShield;
        SceneManager.sceneLoaded += FindShieldUIController;
    }

    private void ResetShield()
    {
        shield.SetActive(false);
        OnShieldActivation(false);
    }

    private void UnlockShield()
    {
        if (!isShieldUnlocked)
        {
            isShieldUnlocked = true;
            audioPlayer.PlayOneShot(shieldUnlocked);
            shieldUI.gameObject.SetActive(true);
            shieldUI.EnableTutorial();
            shieldUI.OnShieldIsCharged += EnableShieldActivation;
        }
    }

    private void FindShieldUIController(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            isShieldUnlocked = false;
            shieldUI = FindObjectOfType<ShieldUIManager>();
            shieldUI.gameObject.SetActive(isShieldUnlocked);
            shieldIsCharged = false;
            shieldUI.OnShieldIsCharged += EnableShieldActivation;
            AfterDeathOptions.instance.OnTryAgainNow += ResetShield;
            AfterDeathOptions.instance.OnSkip += ResetShield;
            shieldController.gameObject.SetActive(false);
        }
    }

    private void EnableShieldActivation()
    {
        shieldIsCharged = true;
    }

    private void Update()
    {
        if (Input.GetButtonDown(activateShieldButton) && shieldIsCharged)
        {
            EnableShield();
            shieldIsCharged = false;
            audioPlayer.PlayOneShot(shieldEnabled);
            StartCoroutine(DisableShieldAfterDepletion());
        }
    }

    private void EnableShield()
    {
        shield.SetActive(true);
        OnShieldActivation(true);
    }

    private IEnumerator DisableShieldAfterDepletion()
    {
        yield return new WaitForSecondsRealtime(timeToDepleteShield);
        shield.SetActive(false);
        OnShieldActivation(false);
        DropBullets();
    }

    private void DropBullets()
    {
        byte amountOfBulletsToDrop = shieldController.GetAmountOfBulletsAbsorbedAndReset();
        var bulletDrop = Instantiate(bulletDropPrefab, transform.position, Quaternion.identity);
        bulletDrop.GetComponent<BulletDrop>().SetAmountOfBulletsToDrop(amountOfBulletsToDrop);
    }
}
