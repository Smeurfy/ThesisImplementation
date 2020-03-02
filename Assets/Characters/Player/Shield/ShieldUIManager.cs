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

    private byte damageDealt = 0;
    private bool isFullyCharged = false;
    private bool isShieldBeingUsed = false;
    private AudioSource audioPlayer;
    private Animator animator;

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
        SceneManager.sceneUnloaded += DereferenceEnemyTakeDamage;
    }

    private void Update()
    {
        if(isShieldBeingUsed)
        {
            timePassed =  (timePassed + Time.deltaTime );
            currentCharge.fillAmount = Mathf.Lerp(1, 0, timePassed * .33f);
        }
    }

    private void StartCountdown(bool isStarting)
    {
        if(isStarting)
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
        ResetBarProgress();
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

    private void ChargeShieldWithDamageDealt(int damageDealt)
    {
        if(!isFullyCharged)
        {
            this.damageDealt += (byte) damageDealt;
            CheckIfShieldReady();
            UpdateChargeBar();
        }
    }

    private void CheckIfShieldReady()
    {
        if(damageDealt >= damageToDealtToChargeShield)
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
        if(gameObject.activeSelf)
        { 
            incrementCharge.fillAmount = GetChargeAsPercentage();
            StartCoroutine(UpdateDamageVisualizationBar());
        }
    }

    private void ResetBarProgress()
    {
        incrementCharge.fillAmount = GetChargeAsPercentage();
        currentCharge.fillAmount = GetChargeAsPercentage();
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
        return (float) damageDealt / damageToDealtToChargeShield;
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
}
