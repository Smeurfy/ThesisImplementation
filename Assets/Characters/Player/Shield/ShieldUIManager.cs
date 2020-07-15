using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Thesis.Enemy;
using System.Collections;

public class ShieldUIManager : MonoBehaviour
{
    public event Action OnShieldIsCharged = delegate { };

    [SerializeField] private byte damageToDealtToChargeShield = 30;
    [SerializeField] private AudioClip shieldCharged;
    [SerializeField] private Image currentCharge;
    [SerializeField] private Image incrementCharge;
    [SerializeField] private GameObject shieldTutorial;
    [SerializeField] private Color shieldDepletingColor;

    private float damageDealt = 0;
    private bool isFullyCharged = false;
    private bool isShieldBeingUsed = false;
    private AudioSource audioPlayer;
    private Animator animator;

    private float shieldBeforeChallenge = 0;

    private const string ANIM_CHARGED = "charged";
    private float timePassed = 0f;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        shieldTutorial.SetActive(false);
    }

    private void Start()
    {
        damageDealt = 0;
        GameManager.instance.GetPlayerReference().GetComponent<ShieldManager>().OnShieldActivation += StartCountdown;
        GameObject.Find("player").GetComponent<PlayerHealthSystem>().OnPlayerDied += UndoShieldUnlock;
        ChargeShieldWithDamageDealt(50); // charges the shield so it can be used once unlocked
        SceneManager.sceneUnloaded += DereferenceEnemyTakeDamage;
    }

    private void UndoShieldUnlock()
    {
        bool result = FindObjectOfType<ScoreManager>().DefeatedRoomsToUnlockShield();
        if (!result)
        {
            try
            {
                gameObject.SetActive(false);
            }
            catch (MissingReferenceException) {
                //bah
             }

            ShieldManager.isShieldUnlocked = false;
        }

    }

    private void Update()
    {
        if (isShieldBeingUsed)
        {
            timePassed = (timePassed + Time.deltaTime);
            currentCharge.fillAmount = Mathf.Lerp(1, 0, timePassed * .33f);
        }
    }

    private void StartCountdown(bool isStarting)
    {
        if (isStarting)
        {
            currentCharge.color = shieldDepletingColor;
            isShieldBeingUsed = true;
        }
        else
        {
            isShieldBeingUsed = false;
            CanChargeShield();
        }
    }

    private void OnEnable()
    {
        Debug.Log("adsfijklçgbhipuawdegjrsçBHINOPUÇADEFGJRSW<");
        CheckIfShieldReady();
        // ResetBarProgress();
        EnemyHealthSystem.OnEnemyTakeDamage += ChargeShieldWithDamageDealt;
    }

    private void OnDisable()
    {
        EnemyHealthSystem.OnEnemyTakeDamage -= ChargeShieldWithDamageDealt;
    }

    private void CanChargeShield()
    {
        isFullyCharged = false;
        damageDealt = 0;
        timePassed = 0;
        ResetBarProgress();
    }

    private void ChargeShieldWithDamageDealt(float damageDealt)
    {
        if (!isFullyCharged)
        {
            this.damageDealt += damageDealt;
            CheckIfShieldReady();
            UpdateChargeBar();
        }
    }

    private void CheckIfShieldReady()
    {
        if (damageDealt >= damageToDealtToChargeShield)
        {
            isFullyCharged = true;
            animator.SetBool(ANIM_CHARGED, true);
            audioPlayer.PlayOneShot(shieldCharged);
            currentCharge.color = Color.yellow;
            OnShieldIsCharged();
        }
    }

    private void UpdateChargeBar()
    {
        if (gameObject.activeSelf)
        {
            incrementCharge.fillAmount = GetChargeAsPercentage();
            StartCoroutine(UpdateDamageVisualizationBar());
        }
    }

    private void ResetBarProgress()
    {
        incrementCharge.fillAmount = GetChargeAsPercentage();
        currentCharge.fillAmount = GetChargeAsPercentage();
        Debug.Log("bgbgbgbgbbgbg");

        currentCharge.color = Color.green;
        animator.SetBool(ANIM_CHARGED, false);
    }

    private IEnumerator UpdateDamageVisualizationBar()
    {
        yield return new WaitForSecondsRealtime(.5f);
        currentCharge.fillAmount = GetChargeAsPercentage();
    }

    private float GetChargeAsPercentage()
    {
        return (float)damageDealt / damageToDealtToChargeShield;
    }

    private void DereferenceEnemyTakeDamage(Scene loadedScene)
    {
        if (loadedScene.buildIndex == GameManager.instance.GetMainGameSceneNumber())
        {
            EnemyHealthSystem.OnEnemyTakeDamage -= ChargeShieldWithDamageDealt;
            GameManager.instance.GetPlayerReference().GetComponent<ShieldManager>().OnShieldActivation -= StartCountdown;
        }
    }

    public void EnableTutorial()
    {
        shieldTutorial.SetActive(true);
    }

    public void DisableTutorial()
    {
        shieldTutorial.SetActive(false);
    }

    public void SubscribeToRoom()
    {
        shieldBeforeChallenge = currentCharge.fillAmount;
    }

    public void UndoShieldCharge()
    {
        Debug.Log("AHAHAHAH");
        damageDealt = 0;
        isFullyCharged = false;
        animator.SetBool(ANIM_CHARGED, false);
        currentCharge.color = Color.green;
        ChargeShieldWithDamageDealt(shieldBeforeChallenge * 30);
    }
}
